#define Enable_Jitter
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CSharpGL;
using ImGui.OSAbstraction.Graphics;
using ImGui.Rendering;
using SixLabors.ImageSharp.PixelFormats;

namespace ImGui.OSImplementation.Shared
{
    public class OpenGLOffscreenRenderer
    {
        static float[] GetViewMatrix(int clientWidth, int clientHeight)
        {
            var right = new Vector3(1, 0, 0);//+x
            var up = new Vector3(0, 1, 0);//+y
            var forward = new Vector3(0, 0, 1);//+z
            //center of client area
            var eye = new Vector3(0, 0, 0);
            var row4 = new Vector3(
                Vector3.Dot(-right, eye),
                Vector3.Dot(-up, eye),
                Vector3.Dot(-forward, eye)
            );
            float[] viewMatrix = new float[16]
            {
                right.X,    right.Y,    right.Z,    0,
                up.X,       up.Y,       up.Z,       0,
                forward.X,  forward.Y,  forward.Z,  0,
                row4.X,     row4.Y,     row4.Z,     1.0f
            };
            return viewMatrix;
        }
        
        static readonly Point[] JITTER_PATTERN =
        {
            new Point (- 1 / 12.0, -5 / 12.0),
            new Point( 1 / 12.0,  1 / 12.0),
            new Point( 3 / 12.0, -1 / 12.0),
            new Point( 5 / 12.0,  5 / 12.0),
            new Point( 7 / 12.0, -3 / 12.0),
            new Point( 9 / 12.0,  3 / 12.0),
        };

        internal byte[] DrawMeshToTexture(Mesh mesh, TextMesh textMesh, int width, int height)
        {
            //**Key Point 1** check scissor test:
            //The initially scissor-test rectangle's size is the size of the window client area.
            GL.Scissor(-4096, -4096, 8192, 8192);//reset scissor rect to a very big one

            //create texture that will be rendered onto
            ITexture texture = new OpenGLTexture();
            texture.LoadImage(new Rgba32[width * height], width, height);
            GL.BindTexture(GL.GL_TEXTURE_2D, 0);
            
            Utility.CheckGLError();
            //create frame buffer
            uint[] framebuffers = {0};
            GL.GenFramebuffers(1, framebuffers);
            uint framebuffer = framebuffers[0];
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, framebuffer);
            GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT,
                GL.GL_TEXTURE_2D, (uint) texture.GetNativeTextureId(), 0);
            if (GL.CheckFramebufferStatus(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
                throw new Exception("Framebuffer is not complete.");
            }

            //**Key point 2** Be sure the state of a framebuffer is expected.
            //Check if framebuffer need to be cleared to make it into an expected state.
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);

            Utility.CheckGLError();
            float l = 0;
            float r = width;
            float x = 0;
            float b = height;
            float n = -0.1f;
            float f = 100.0f;
            float[] ortho_projection = new float[16]
            {
                2.0f/(r-l),   0.0f,         0.0f,           0.0f,
                0.0f,         2.0f/(x-b),   0.0f,           0.0f,
                0.0f,         0.0f,         2.0f/(n-f),     0.0f,
                (r+l)/(l-r),  (x+b)/(b-x),  (f+n)/(n-f),    1.0f,
            };
            var viewMatrix = GetViewMatrix(width, height);

            if (mesh!=null && !mesh.IsEmpty)
            {
                List<DrawCommand> commandBuffer = mesh.CommandBuffer;
                VertexBuffer vertexBuffer = mesh.VertexBuffer;
                IndexBuffer indexBuffer = mesh.IndexBuffer;

                // **Key Point 3** Blending states have an impact on rendering result.
                // Check them before **every** draw-call on a framebuffer, **especially the first one**.
                GL.Enable(GL.GL_BLEND);
                GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
                GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);//doing regular alpha blending
                GL.Disable(GL.GL_CULL_FACE);
                GL.Disable(GL.GL_DEPTH_TEST);
                GL.DepthFunc(GL.GL_NEVER);

                // Setup viewport, orthographic projection matrix
                GL.Viewport(0, 0, width, height);

                // Setup view transformation
                OpenGLMaterial.shapeMaterial.program.Bind();
                OpenGLMaterial.shapeMaterial.program.SetUniformMatrix4("ViewMtx", viewMatrix);
                OpenGLMaterial.shapeMaterial.program.SetUniformMatrix4("ProjMtx", ortho_projection);

                // Send vertex and index data
                GL.BindVertexArray(OpenGLMaterial.shapeMaterial.VaoHandle);
                GL.BindBuffer(GL.GL_ARRAY_BUFFER, OpenGLMaterial.shapeMaterial.VboHandle);
                GL.BufferData(GL.GL_ARRAY_BUFFER, vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(),
                    vertexBuffer.Pointer,
                    GL.GL_STREAM_DRAW);
                GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, OpenGLMaterial.shapeMaterial.EboHandle);
                GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.Count * Marshal.SizeOf<DrawIndex>(),
                    indexBuffer.Pointer, GL.GL_STREAM_DRAW);

                Utility.CheckGLError();

                // Draw
                var indexBufferOffset = IntPtr.Zero;
                foreach (var drawCmd in commandBuffer)
                {
                    if (drawCmd.ElemCount == 0 || drawCmd.ClipRect.IsEmpty)
                    {
                        continue;
                    }

                    var clipRect = drawCmd.ClipRect;
                    if (drawCmd.TextureData != null)
                    {
                        GL.ActiveTexture(GL.GL_TEXTURE0);
                        GL.BindTexture(GL.GL_TEXTURE_2D, (uint) drawCmd.TextureData.GetNativeTextureId());
                    }

                    GL.Scissor((int) clipRect.X, (int) (height - clipRect.Height - clipRect.Y), (int) clipRect.Width,
                        (int) clipRect.Height);
                    GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);
                    indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());

                    Utility.CheckGLError();
                }
            }
            
            //check TextMesh
            if (textMesh!=null && !textMesh.IsEmpty)
            {
                //create texture for glyph
                OpenGLTexture glyphOpenGLTexture = new OpenGLTexture();
                glyphOpenGLTexture.LoadImage(new Rgba32[width * height], width, height);
                uint glyphTexture = (uint)glyphOpenGLTexture.GetNativeTextureId();

                //create frame buffer for glyph
                uint[] textMeshFrameBuffers = {0};
                GL.GenFramebuffers(1, textMeshFrameBuffers);
                uint glyphFrameBuffer = textMeshFrameBuffers[0];

                //attach textures to glyph framebuffer
                GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, glyphFrameBuffer);
                GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT,
                    GL.GL_TEXTURE_2D, glyphTexture, 0);
                if (GL.CheckFramebufferStatus(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
                {
                    throw new Exception("Framebuffer for glyph is not complete.");
                }

                var quadMesh = new QuadMesh(width, height);

                var commandBuffer = textMesh.Commands;
                VertexBuffer vertexBuffer = textMesh.VertexBuffer;
                IndexBuffer indexBuffer = textMesh.IndexBuffer;

                GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, glyphFrameBuffer);
                GL.Viewport(0, 0, width, height);
                GL.ClearColor(0, 0, 0, 0);
                GL.Clear(GL.GL_COLOR_BUFFER_BIT);

                // setup render state
                GL.Enable(GL.GL_BLEND);
                GL.BlendEquation(GL.GL_FUNC_ADD_EXT);

                // setup viewport, orthographic projection and view transformation
                var glyphMaterial = OpenGLMaterial.glyphMaterial;
                glyphMaterial.program.Bind();
                glyphMaterial.program.SetUniformMatrix4("ViewMtx", viewMatrix);
                glyphMaterial.program.SetUniformMatrix4("ProjMtx", ortho_projection);

                // send mesh data
                GL.BindVertexArray(glyphMaterial.VaoHandle);
                GL.BindBuffer(GL.GL_ARRAY_BUFFER, glyphMaterial.VboHandle);
                GL.BufferData(GL.GL_ARRAY_BUFFER, 
                    vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), vertexBuffer.Pointer,
                    GL.GL_STREAM_DRAW);
                GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, glyphMaterial.EboHandle);
                GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER,
                    indexBuffer.Count * Marshal.SizeOf<DrawIndex>(), indexBuffer.Pointer,
                    GL.GL_STREAM_DRAW);
                Utility.CheckGLError();

                //draw
                GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
                GL.BlendFunc(GL.GL_ONE, GL.GL_ONE);
                var indexBufferOffset = IntPtr.Zero;
#if Enable_Jitter
                for (int j = 0; j < JITTER_PATTERN.Length; j++)
                {
                    var offset = JITTER_PATTERN[j];
                    glyphMaterial.program.SetUniform("offset", offset.x, offset.y);
                    indexBufferOffset = IntPtr.Zero;//reset to redraw with a different offset
                    if(j % 2 == 0)
                    {
                        glyphMaterial.program.SetUniform("color", j == 0 ? 1 : 0, j == 2 ? 1 : 0, j == 4 ? 1 : 0, 0);
                    }
#else
                    glyphMaterial.program.SetUniform("offset", 0.0f, 0.0f);
                    glyphMaterial.program.SetUniform("color", 1.0f, 1.0f, 1.0f, 0.0f);
#endif

                    // Draw text mesh 
                    foreach (var drawCommand in commandBuffer)
                    {
                        var clipRect = drawCommand.ClipRect;
                        GL.Scissor((int) clipRect.X, (int) (height - clipRect.Height - clipRect.Y), (int) clipRect.Width, (int) clipRect.Height);

                        GL.DrawElements(GL.GL_TRIANGLES, drawCommand.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);

                        Utility.CheckGLError();
                        indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCommand.ElemCount * Marshal.SizeOf<DrawIndex>());
                    }
#if Enable_Jitter
                }
#endif
                Utility.CheckGLError();

                // second draw to the framebuffer as a quad
                GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, framebuffer);
                var textMaterial = OpenGLMaterial.textMaterial;
                GL.BlendFunc(GL.GL_ZERO, GL.GL_SRC_COLOR);
                textMaterial.program.Bind();
                textMaterial.program.SetUniform("color", 0.0f, 0.0f, 0.0f, 0.0f);
                GL.Disable(GL.GL_SCISSOR_TEST);
                GL.ActiveTexture(GL.GL_TEXTURE0);
                GL.BindTexture(GL.GL_TEXTURE_2D, glyphTexture);
                GL.BindVertexArray(textMaterial.VaoHandle);
                GL.BindBuffer(GL.GL_ARRAY_BUFFER, textMaterial.VboHandle);
                GL.BufferData(GL.GL_ARRAY_BUFFER, quadMesh.VertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), quadMesh.VertexBuffer.Pointer, GL.GL_STREAM_DRAW);
                GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, textMaterial.EboHandle);
                GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, quadMesh.IndexBuffer.Count * Marshal.SizeOf<DrawIndex>(), quadMesh.IndexBuffer.Pointer, GL.GL_STREAM_DRAW);
                GL.DrawElements(GL.GL_TRIANGLES, quadMesh.CommandBuffer[0].ElemCount, GL.GL_UNSIGNED_INT, IntPtr.Zero);

                // first draw to glyph framebuffer
                GL.DeleteFramebuffers(1, textMeshFrameBuffers);
                glyphOpenGLTexture.Dispose();
            }

            //read pixels from the framebuffer
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, framebuffer);
            Utility.CheckGLError();
            var pixels = new byte[width * height * 4];
            GL.ReadPixels(0, 0, width, height, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
            
            Utility.CheckGLError();

            //dispose OpenGL resources used when rendering mesh and textMesh
            texture.Dispose();
            GL.DeleteFramebuffers(1, framebuffers);
            Utility.CheckGLError();

            return pixels;
        }
    }
}