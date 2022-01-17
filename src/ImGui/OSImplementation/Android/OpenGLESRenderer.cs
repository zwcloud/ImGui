#define Enable_Jitter
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Window;
using ImGui.OSImplementation.Shared;
using CSharpGL;
using ImGui.Rendering;

namespace ImGui.OSImplementation.Android
{
    internal partial class OpenGLESRenderer : IRenderer
    {
        //Helper for some GL functions
        private static readonly int[] IntBuffer = { 0, 0, 0, 0 };
        private static readonly float[] FloatBuffer = { 0, 0, 0, 0 };
        
        //framebuffer
        private readonly uint[] framebuffers = { 0 };
        private readonly uint[] textures = { 0 };

        static uint framebuffer = 0;
        static uint framebufferColorTexture = 0;
        static QuadMesh quadMesh;

        static readonly Point[] JITTER_PATTERN =
        {
            new Point (- 1 / 12.0, -5 / 12.0),
            new Point( 1 / 12.0,  1 / 12.0),
            new Point( 3 / 12.0, -1 / 12.0),
            new Point( 5 / 12.0,  5 / 12.0),
            new Point( 7 / 12.0, -3 / 12.0),
            new Point( 9 / 12.0,  3 / 12.0),
        };

        static float[] GetViewMatrix()
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

        public OpenGLESRenderer(IWindow window)
        {
            Init(window.Pointer, window.ClientSize);
        }

        public void Init(IntPtr windowHandle, Size size)
        {
            //CreateOpenGLContext((IntPtr)windowHandle);//done in Xamarin.Android
            //InitGLEW();//done in Xamarin.Android

            OpenGLMaterial.InitCommonMaterials();

            // Other state
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.DepthFunc(GL.GL_NEVER);
            GL.Enable(GL.GL_SCISSOR_TEST);
            
            InitializeTextRenderResources(size);
        }

        public void SetRenderingWindow(IWindow window)
        {
            //for android, only one unique native window.
            Debug.Assert(window == Application.ImGuiContext.WindowManager.MainForm.NativeWindow);
        }

        public IWindow GetRenderingWindow()
        {
            return Application.ImGuiContext.WindowManager.MainForm.NativeWindow;
        }

        public void OnSizeChanged(Size size)
        {
            RebuildTextureRenderResources(size);
        }

        public void Bind()
        {
            //TODO determine if android or other future OpenGLES renders need to Egl.MakeCurrent()
        }
        
        private void CreateTextFramebuffer(Size size)
        {
            Utility.CheckGLError();
            //set-up framebuffer
            GL.GenFramebuffers(1, framebuffers);
            GL.GenTextures(1, textures);
            framebuffer = framebuffers[0];
            framebufferColorTexture = textures[0];
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, framebuffer);

            //attach color texture to the framebuffer
            Utility.CheckGLError();
            GL.BindTexture(GL.GL_TEXTURE_2D, framebufferColorTexture);
            Utility.CheckGLError();
            GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA,
                (int)size.Width, (int)size.Height, 0,
                GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, IntPtr.Zero);
            Utility.CheckGLError();
            GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT, GL.GL_TEXTURE_2D,
                framebufferColorTexture, 0);
            Utility.CheckGLError();
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_NEAREST);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_NEAREST);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP_TO_EDGE);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP_TO_EDGE);
            Utility.CheckGLError();
            GL.GetFramebufferAttachmentParameteriv(GL.GL_FRAMEBUFFER_EXT,
                GL.GL_COLOR_ATTACHMENT0_EXT, GL.GL_FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE, IntBuffer);
            Utility.CheckGLError();
            var alphaBits = IntBuffer[0];
            if(alphaBits != 8)
            {
                throw new Exception("Framebuffer format is not R8G8B8A8.");
            }

            //check if framebuffer is complete
            if (GL.CheckFramebufferStatus(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
                throw new Exception("Framebuffer is not complete.");
            }
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, 0);

            Debug.Assert(GL.IsFramebufferEXT(framebuffer));
            Debug.Assert(GL.IsTexture(framebufferColorTexture) == GL.GL_TRUE);
        }

        private void DeleteTextFrameBuffer()
        {
            GL.DeleteFramebuffers(1, framebuffers);
            GL.DeleteTextures(1, textures);

            Debug.Assert(!GL.IsFramebufferEXT(framebuffer));
            Debug.Assert(GL.IsTexture(framebufferColorTexture) == GL.GL_FALSE);
        }

        private void InitializeTextRenderResources(Size viewportSize)
        {
            CreateTextFramebuffer(viewportSize);
            quadMesh = new QuadMesh();
            Utility.CheckGLError();
        }

        private void RebuildTextureRenderResources(Size viewportSize)
        {
            //delete old framebuffer
            DeleteTextFrameBuffer();

            //create new framebuffer
            CreateTextFramebuffer(viewportSize);
        }

        public void Clear(Color clearColor)
        {
            GL.ClearColor((float)clearColor.R, (float)clearColor.G, (float)clearColor.B, (float)clearColor.A);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
        }

        public void DrawMeshes(int width, int height, (Mesh shapeMesh, Mesh imageMesh, TextMesh textMesh) meshes)
        {
            DrawMesh(OpenGLMaterial.shapeMaterial, meshes.shapeMesh, width, height);
            DrawMesh(OpenGLMaterial.imageMaterial, meshes.imageMesh, width, height);

            DrawTextMesh(meshes.textMesh, width, height);
        }

        private static void DrawMesh(OpenGLMaterial material, Mesh mesh, int width, int height)
        {
            if (mesh.IsEmpty)
            {
                return;
            }
            List<DrawCommand> commandBuffer = mesh.CommandBuffer;
            VertexBuffer vertexBuffer = mesh.VertexBuffer;
            IndexBuffer indexBuffer = mesh.IndexBuffer;
            
            Utility.CheckGLError();
            // Backup GL state
            GL.GetIntegerv(GL.GL_CURRENT_PROGRAM, IntBuffer); int last_program = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_TEXTURE_BINDING_2D, IntBuffer); int last_texture = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_ACTIVE_TEXTURE, IntBuffer); int last_active_texture = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_ARRAY_BUFFER_BINDING, IntBuffer); int last_array_buffer = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_ELEMENT_ARRAY_BUFFER_BINDING, IntBuffer); int last_element_array_buffer = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_VERTEX_ARRAY_BINDING, IntBuffer);int last_vertex_array = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_BLEND_SRC_RGB, IntBuffer); int last_blend_src_grb = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_BLEND_SRC_ALPHA, IntBuffer); int last_blend_src_alpha = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_BLEND_DST_RGB, IntBuffer); int last_blend_dst_rgb = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_BLEND_DST_ALPHA, IntBuffer); int last_blend_dst_alpha = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_RGB, IntBuffer); int last_blend_equation_rgb = IntBuffer[0];
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_ALPHA, IntBuffer);int last_blend_equation_alpha = IntBuffer[0];
            
            Utility.CheckGLError();
            GL.GetIntegerv(GL.GL_VIEWPORT, IntBuffer); Rect last_viewport = new Rect(IntBuffer[0], IntBuffer[1], IntBuffer[2], IntBuffer[3]);
            uint last_enable_blend = GL.IsEnabled(GL.GL_BLEND);
            uint last_enable_cull_face = GL.IsEnabled(GL.GL_CULL_FACE);
            uint last_enable_depth_test = GL.IsEnabled(GL.GL_DEPTH_TEST);
            uint last_enable_scissor_test = GL.IsEnabled(GL.GL_SCISSOR_TEST);
            GL.GetIntegerv(GL.GL_SCISSOR_BOX, IntBuffer);
            int last_sessor_rect_x = IntBuffer[0];
            int last_sessor_rect_y = IntBuffer[1];
            int last_sessor_rect_width = IntBuffer[2];
            int last_sessor_rect_height = IntBuffer[3];
            
            Utility.CheckGLError();
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled
            GL.Enable(GL.GL_BLEND);
            Utility.CheckGLError();
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            Utility.CheckGLError();
            GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
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
            var viewMatrix = GetViewMatrix();
            material.program.Bind();
            material.program.SetUniformMatrix4("ViewMtx", viewMatrix);
            material.program.SetUniformMatrix4("ProjMtx", ortho_projection);

            // Send vertex and index data
            GL.BindVertexArray(material.VaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, material.VboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER, vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), vertexBuffer.Pointer, GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, material.EboHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.Count * Marshal.SizeOf<DrawIndex>(), indexBuffer.Pointer, GL.GL_STREAM_DRAW);

            Utility.CheckGLError();

            // Draw
            var indexBufferOffset = IntPtr.Zero;
            foreach (var drawCmd in commandBuffer)
            {
                var clipRect = drawCmd.ClipRect;
                if (drawCmd.TextureData != null)
                {
                    GL.ActiveTexture(GL.GL_TEXTURE0);
                    GL.BindTexture(GL.GL_TEXTURE_2D, (uint)drawCmd.TextureData.GetNativeTextureId());
                }
                GL.Scissor((int) clipRect.X, (int) (height - clipRect.Height - clipRect.Y), (int) clipRect.Width, (int) clipRect.Height);
                GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);
                indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount*Marshal.SizeOf<DrawIndex>());

                Utility.CheckGLError();
            }

            // Restore modified GL state
            GL.UseProgram((uint)last_program);
            GL.ActiveTexture((uint)last_active_texture);
            GL.BindTexture(GL.GL_TEXTURE_2D, (uint)last_texture);
            GL.BindVertexArray((uint)last_vertex_array);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, (uint)last_array_buffer);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, (uint)last_element_array_buffer);
            GL.BlendEquationSeparate((uint)last_blend_equation_rgb, (uint)last_blend_equation_alpha);
            GL.BlendFuncSeparate((uint)last_blend_src_grb, (uint)last_blend_dst_rgb, (uint)last_blend_src_alpha, (uint)last_blend_dst_alpha);
            if (last_enable_blend == GL.GL_TRUE) GL.Enable(GL.GL_BLEND); else GL.Disable(GL.GL_BLEND);
            if (last_enable_cull_face == GL.GL_TRUE) GL.Enable(GL.GL_CULL_FACE); else GL.Disable(GL.GL_CULL_FACE);
            if (last_enable_depth_test == GL.GL_TRUE) GL.Enable(GL.GL_DEPTH_TEST); else GL.Disable(GL.GL_DEPTH_TEST);
            if (last_enable_scissor_test == GL.GL_TRUE) GL.Enable(GL.GL_SCISSOR_TEST); else GL.Disable(GL.GL_SCISSOR_TEST);
            GL.Viewport((int)last_viewport.X, (int)last_viewport.Y, (int)last_viewport.Width, (int)last_viewport.Height);
            GL.Scissor(last_sessor_rect_x, last_sessor_rect_y, last_sessor_rect_width, last_sessor_rect_height);
        }

        private void DrawTextMesh(TextMesh textMesh, int width, int height)
        {
            if (textMesh.IsEmpty)
            {
                return;
            }
            var glyphMaterial = OpenGLMaterial.glyphMaterial;
            var textMaterial = OpenGLMaterial.textMaterial;

            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, framebuffer);
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT);

            glyphMaterial.program.Bind();
            float L = 0;
            float R = 0 + width;
            float T = 0;
            float B = 0 + height;
            float[] ortho_projection = new float[16]
            {
                2.0f/(R-L),   0.0f,         0.0f,   0.0f,
                0.0f,         2.0f/(T-B),   0.0f,   0.0f,
                0.0f,         0.0f,        -1.0f,   0.0f,
                (R+L)/(L-R),  (T+B)/(B-T),  0.0f,   1.0f,
            };
            var viewMatrix = GetViewMatrix();
            glyphMaterial.program.Bind();
            glyphMaterial.program.SetUniformMatrix4("ViewMtx", viewMatrix);
            glyphMaterial.program.SetUniformMatrix4("ProjMtx", ortho_projection);

            Utility.CheckGLError();

            // Send vertex data
            GL.BindVertexArray(glyphMaterial.VaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, glyphMaterial.VboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER, textMesh.VertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), textMesh.VertexBuffer.Pointer, GL.GL_STATIC_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, glyphMaterial.EboHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, textMesh.IndexBuffer.Count * Marshal.SizeOf<DrawIndex>(), textMesh.IndexBuffer.Pointer, GL.GL_STATIC_DRAW);

            Utility.CheckGLError();

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
                foreach (var drawCmd in textMesh.Commands)
                {
                    var clipRect = drawCmd.ClipRect;
                    GL.Scissor((int) clipRect.X, (int) (height - clipRect.Height - clipRect.Y), (int) clipRect.Width, (int) clipRect.Height);
                    // Draw text mesh 
                    GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);

                    Utility.CheckGLError();
                    indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
                }
#if Enable_Jitter
            }
#endif

            //Draw framebuffer texture to screen as a quad,  with the textMaterial applying sub-pixel anti-aliasing
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, 0);
            GL.BlendFunc(GL.GL_ZERO, GL.GL_SRC_COLOR);
            //VERY IMPORTANT NOTE: the frame-buffer must be cleared to a color with non-zero alpha,
            //NOTE clear color's alpha channel is the transparency of the drawn text
            //NOTE A clear color with alpha 0 will make the drawn text invisible!!
            textMaterial.program.Bind();
            textMaterial.program.SetUniform("color", 0.0f, 0.0f, 0.0f, 0.0f);
            GL.Disable(GL.GL_SCISSOR_TEST);
            GL.ActiveTexture(GL.GL_TEXTURE0);
            GL.BindTexture(GL.GL_TEXTURE_2D, framebufferColorTexture);
            GL.BindVertexArray(textMaterial.VaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, textMaterial.VboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER, quadMesh.VertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), quadMesh.VertexBuffer.Pointer, GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, textMaterial.EboHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, quadMesh.IndexBuffer.Count * Marshal.SizeOf<DrawIndex>(), quadMesh.IndexBuffer.Pointer, GL.GL_STREAM_DRAW);
            GL.DrawElements(GL.GL_TRIANGLES, quadMesh.CommandBuffer[0].ElemCount, GL.GL_UNSIGNED_INT, IntPtr.Zero);
            Utility.CheckGLError();
        }

        public void Unbind()
        {
            //TODO determine if android or other future OpenGLES renders need to Egl.MakeCurrent()
        }

        public void ShutDown()
        {
            OpenGLMaterial.ShutDownCommonMaterials();
        }

        public byte[] GetRawBackBuffer(out int width, out int height)
        {
            GL.GetIntegerv(GL.GL_VIEWPORT, IntBuffer);
            int viewportX = IntBuffer[0];
            int viewportY = IntBuffer[1];
            int viewportWidth = width = IntBuffer[2];
            int viewportHeight = height = IntBuffer[3];
            var pixels = new byte[viewportWidth * viewportHeight * 4];
            GL.ReadPixels(viewportX, viewportY, viewportWidth, viewportHeight, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
            return pixels;
        }
    }
}
