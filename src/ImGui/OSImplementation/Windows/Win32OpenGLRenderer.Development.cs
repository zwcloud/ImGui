#define Enable_Jitter
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CSharpGL;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSImplementation.Shared;
using SixLabors.ImageSharp.PixelFormats;

namespace ImGui.OSImplementation.Windows
{
    internal partial class Win32OpenGLRenderer
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

        #region Draw Mesh
        ITexture textureForDrawMesh = null;
        uint[] frameBuffersForDrawMesh = {0};
        uint frameBufferForDrawMesh = 0;

        public void StartDrawMeshToImage(int width, int height)
        {
            //create texture
            textureForDrawMesh = new OpenGLTexture();
            textureForDrawMesh.LoadImage(new Rgba32[width * height], width, height);

            //create frame buffer
            GL.GenFramebuffers(1, frameBuffersForDrawMesh);
            frameBufferForDrawMesh = frameBuffersForDrawMesh[0];
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, frameBufferForDrawMesh);
            GL.BindTexture(GL.GL_TEXTURE_2D, framebufferColorTexture);
            GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT,
                GL.GL_TEXTURE_2D, (uint) textureForDrawMesh.GetNativeTextureId(), 0);
            if (GL.CheckFramebufferStatus(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
                throw new Exception("Framebuffer is not complete.");
            }
        }

        public byte[] DrawMeshToImage(
            int width, int height,
            Mesh mesh, OpenGLMaterial material)
        {
            List<DrawCommand> commandBuffer = mesh.CommandBuffer;
            if (commandBuffer.Count == 0 || commandBuffer[0].ElemCount == 0)
            {
                return null;
            }

            VertexBuffer vertexBuffer = mesh.VertexBuffer;
            IndexBuffer indexBuffer = mesh.IndexBuffer;


            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled
            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            GL.BlendFunc(GL.GL_ONE, GL.GL_ONE);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.DepthFunc(GL.GL_NEVER);
            GL.Enable(GL.GL_SCISSOR_TEST);

            // Setup viewport, orthographic projection matrix
            GL.Viewport(0, 0, width, height);
            float L = 0;
            float R = width;
            float T = 0;
            float B = height;
            float N = -0.1f;
            float F = 100.0f;
            float[] ortho_projection = new float[16]
            {
                2.0f/(R-L),   0.0f,         0.0f,           0.0f,
                0.0f,         2.0f/(T-B),   0.0f,           0.0f,
                0.0f,         0.0f,         2.0f/(N-F),     0.0f,
                (R+L)/(L-R),  (T+B)/(B-T),  (F+N)/(N-F),    1.0f,
            };

            // Setup view transformation
            var viewMatrix = GetViewMatrix(width, height);
            material.program.Bind();
            material.program.SetUniformMatrix4("ViewMtx", viewMatrix);
            material.program.SetUniformMatrix4("ProjMtx", ortho_projection);

            // Send vertex and index data
            GL.BindVertexArray(material.VaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, material.VboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER, vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), vertexBuffer.Pointer,
                GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, material.EboHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.Count * Marshal.SizeOf<DrawIndex>(),
                indexBuffer.Pointer, GL.GL_STREAM_DRAW);

            Utility.CheckGLError();

            // Draw
            var indexBufferOffset = IntPtr.Zero;
            foreach (var drawCmd in commandBuffer)
            {
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

            //TODO dispose OpenGL resources

            var pixels = new byte[width * height * 4];
            GL.ReadPixels(0, 0, width, height, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
            return pixels;
        }
        
        public void EndDrawMeshToImage()
        {
            //dispose OpenGL resources
            GL.DeleteFramebuffers(1, frameBuffersForDrawMesh);
            textureForDrawMesh.Dispose();
        }
        #endregion

        #region Draw TextMesh

        //OpenGLTextures
        private OpenGLTexture glyphOpenGLTexture, textOpenGLTexture;

        //frame buffers
        uint[] textMeshFrameBuffers = {0, 0};
        uint glyphFrameBuffer = 0, textFrameBuffer = 0;

        //attachment textures
        uint glyphTexture = 0, textTexture = 0;

        public void StartDrawTextMeshToImage(int width, int height)
        {
            //create textures
            {
                glyphOpenGLTexture = new OpenGLTexture();
                glyphOpenGLTexture.LoadImage(new Rgba32[width * height], width, height);
                glyphTexture = (uint)glyphOpenGLTexture.GetNativeTextureId();
            }
            {
                textOpenGLTexture = new OpenGLTexture();
                textOpenGLTexture.LoadImage(new Rgba32[width * height], width, height);
                textTexture = (uint)textOpenGLTexture.GetNativeTextureId();
            }

            //create frame buffer
            GL.GenFramebuffers(2, textMeshFrameBuffers);
            this.glyphFrameBuffer = textMeshFrameBuffers[0];
            this.textFrameBuffer = textMeshFrameBuffers[1];

            //attach textures to framebuffers
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, this.glyphFrameBuffer);
            GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT,
                GL.GL_TEXTURE_2D, glyphTexture, 0);
            if (GL.CheckFramebufferStatus(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
                throw new Exception("Framebuffer for glyph is not complete.");
            }
            
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, this.textFrameBuffer);
            GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT,
                GL.GL_TEXTURE_2D, textTexture, 0);
            if (GL.CheckFramebufferStatus(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
                throw new Exception("Framebuffer for text is not complete.");
            }
        }

        public byte[] DrawTextMeshToImage(TextMesh textMesh, int width, int height)
        {
            //check TextMesh
            var commandBuffer = textMesh.Commands;
            if (commandBuffer.Count == 0 || commandBuffer[0].ElemCount == 0)
            {
                return null;
            }
            
            // first draw to glyph framebuffer
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, this.glyphFrameBuffer);
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT);

            // setup render state
            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);

            // setup viewport, orthographic projection and view transformation
            GL.Viewport(0, 0, width, height);
            float L = 0;
            float R = width;
            float T = 0;
            float B = height;
            float N = -0.1f;
            float F = 100.0f;
            float[] ortho_projection = new float[16]
            {
                2.0f/(R-L),   0.0f,         0.0f,           0.0f,
                0.0f,         2.0f/(T-B),   0.0f,           0.0f,
                0.0f,         0.0f,         2.0f/(N-F),     0.0f,
                (R+L)/(L-R),  (T+B)/(B-T),  (F+N)/(N-F),    1.0f,
            };
            var viewMatrix = GetViewMatrix(width, height);
            var glyphMaterial = OpenGLMaterial.glyphMaterial;
            glyphMaterial.program.Bind();
            glyphMaterial.program.SetUniformMatrix4("ViewMtx", viewMatrix);
            glyphMaterial.program.SetUniformMatrix4("ProjMtx", ortho_projection);

            // send mesh data
            VertexBuffer vertexBuffer = textMesh.VertexBuffer;
            IndexBuffer indexBuffer = textMesh.IndexBuffer;
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
            GL.Enable(GL.GL_SCISSOR_TEST);
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
                foreach (var drawCmd in textMesh.Commands)
                {
                    var clipRect = drawCmd.ClipRect;
                    GL.Scissor((int) clipRect.X, (int) (height - clipRect.Height - clipRect.Y), (int) clipRect.Width, (int) clipRect.Height);

                    GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);

                    Utility.CheckGLError();
                    indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
                }
#if Enable_Jitter
            }
#endif
            
            // second draw to text framebuffer as a quad, with the textMaterial applying sub-pixel anti-aliasing
            var textMaterial = OpenGLMaterial.textMaterial;
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, textFrameBuffer);
            //change RGB to change the background of text, change Alpha to change the transparency of background
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT);
            //NOTE clear color's alpha channel is the transparency of the drawn text
            //NOTE A clear color with alpha 0 will make the drawn text invisible!!
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

            //read pixels from current bound framebuffer: text framebuffer
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, textFrameBuffer);
            var pixels = new byte[width * height * 4];
            GL.ReadPixels(0, 0, width, height, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
            return pixels;
        }

        public void EndDrawTextMeshToImage()
        {
            GL.DeleteFramebuffers(2, textMeshFrameBuffers);
            this.glyphOpenGLTexture.Dispose();
            this.textOpenGLTexture.Dispose();
        }
        #endregion

    }
}