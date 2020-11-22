using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CSharpGL;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSImplementation.Shared;

namespace ImGui.OSImplementation.Linux
{
    internal partial class LinuxOpenGLRenderer : IRenderer
    {
        //Helper for some GL functions
        private static readonly int[] IntBuffer = { 0, 0, 0, 0 };
        private static readonly float[] FloatBuffer = { 0, 0, 0, 0 };
        private static readonly uint[] UIntBuffer = { 0, 0, 0, 0 };

        public void Init(IntPtr windowHandle, Size size)
        {
            if (!CreateEGLContext())
            {
                Debug.WriteLine("Failed to create EGL context.");
                return;
            }

            var nativeWindow = windowHandle;
            if (!CreateEGLSurface(nativeWindow))
            {
                Debug.WriteLine("Failed to create EGL surface.");
                return;
            }

            OpenGLMaterial.InitCommonMaterials();

            // Other state
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.DepthFunc(GL.GL_NEVER);
            GL.Enable(GL.GL_SCISSOR_TEST);

            Utility.CheckGLESError();
        }
        
        public void Bind()
        {
            //TODO consider Linux usage base on libxcb and GLX
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
            List<DrawCommand> commandBuffer = mesh.CommandBuffer;
            VertexBuffer vertexBuffer = mesh.VertexBuffer;
            IndexBuffer indexBuffer = mesh.IndexBuffer;

            // Backup GL state
            GL.GetIntegerv(GL.GL_CURRENT_PROGRAM, IntBuffer); int last_program = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_TEXTURE_BINDING_2D, IntBuffer); int last_texture = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_ACTIVE_TEXTURE, IntBuffer); int last_active_texture = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_ARRAY_BUFFER_BINDING, IntBuffer); int last_array_buffer = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_ELEMENT_ARRAY_BUFFER_BINDING, IntBuffer); int last_element_array_buffer = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_VERTEX_ARRAY_BINDING, IntBuffer); int last_vertex_array = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_SRC_RGB, IntBuffer); int last_blend_src_rgb = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_SRC_ALPHA, IntBuffer); int last_blend_src_alpha = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_DST_RGB, IntBuffer); int last_blend_dst_rgb = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_DST_ALPHA, IntBuffer); int last_blend_dst_alpha = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_RGB, IntBuffer); int last_blend_equation_rgb = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_ALPHA, IntBuffer); int last_blend_equation_alpha = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_VIEWPORT, IntBuffer); Rect last_viewport = new Rect(IntBuffer[0], IntBuffer[1], IntBuffer[2], IntBuffer[3]);
            Utility.CheckGLESError();
            uint last_enable_blend = GL.IsEnabled(GL.GL_BLEND);
            Utility.CheckGLESError();
            uint last_enable_cull_face = GL.IsEnabled(GL.GL_CULL_FACE);
            Utility.CheckGLESError();
            uint last_enable_depth_test = GL.IsEnabled(GL.GL_DEPTH_TEST);
            Utility.CheckGLESError();
            uint last_enable_scissor_test = GL.IsEnabled(GL.GL_SCISSOR_TEST);
            Utility.CheckGLESError();

            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled
            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.Enable(GL.GL_SCISSOR_TEST);

            // Setup viewport, orthographic projection matrix
            GL.Viewport(0, 0, width, height);
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
            material.program.Bind();
            material.program.SetUniformMatrix4("ProjMtx", ortho_projection);

            // Send vertex and index data
            GL.BindVertexArray(material.VaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, material.VboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER, vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), vertexBuffer.Pointer, GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, material.EboHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.Count * Marshal.SizeOf<DrawIndex>(), indexBuffer.Pointer, GL.GL_STREAM_DRAW);

            Utility.CheckGLESError();

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
                GL.Scissor((int)clipRect.X, (int)(height - clipRect.Height - clipRect.Y), (int)clipRect.Width, (int)clipRect.Height);
                GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);
                indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
            }

            Utility.CheckGLESError();

            // Restore modified GL state
            GL.UseProgram((uint)last_program);
            GL.ActiveTexture((uint)last_active_texture);
            GL.BindTexture(GL.GL_TEXTURE_2D, (uint)last_texture);
            GL.BindVertexArray((uint)last_vertex_array);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, (uint)last_array_buffer);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, (uint)last_element_array_buffer);
            GL.BlendEquationSeparate((uint)last_blend_equation_rgb, (uint)last_blend_equation_alpha);
            GL.BlendFunc((uint)last_blend_src_rgb, (uint)last_blend_dst_rgb);
            GL.BlendFunc((uint)last_blend_src_alpha, (uint)last_blend_dst_alpha);
            if (last_enable_blend == GL.GL_TRUE) GL.Enable(GL.GL_BLEND); else GL.Disable(GL.GL_BLEND);
            if (last_enable_cull_face == GL.GL_TRUE) GL.Enable(GL.GL_CULL_FACE); else GL.Disable(GL.GL_CULL_FACE);
            if (last_enable_depth_test == GL.GL_TRUE) GL.Enable(GL.GL_DEPTH_TEST); else GL.Disable(GL.GL_DEPTH_TEST);
            if (last_enable_scissor_test == GL.GL_TRUE) GL.Enable(GL.GL_SCISSOR_TEST); else GL.Disable(GL.GL_SCISSOR_TEST);
            GL.Viewport((int)last_viewport.X, (int)last_viewport.Y, (int)last_viewport.Width, (int)last_viewport.Height);
        }

        private void DrawTextMesh(TextMesh textMesh, int width, int height)
        {
            // Backup GL state
            GL.GetIntegerv(GL.GL_CURRENT_PROGRAM, IntBuffer); int last_program = IntBuffer[0];
            GL.GetIntegerv(GL.GL_TEXTURE_BINDING_2D, IntBuffer); int last_texture = IntBuffer[0];
            GL.GetIntegerv(GL.GL_ACTIVE_TEXTURE, IntBuffer); int last_active_texture = IntBuffer[0];
            GL.GetIntegerv(GL.GL_ARRAY_BUFFER_BINDING, IntBuffer); int last_array_buffer = IntBuffer[0];
            GL.GetIntegerv(GL.GL_ELEMENT_ARRAY_BUFFER_BINDING, IntBuffer); int last_element_array_buffer = IntBuffer[0];
            GL.GetIntegerv(GL.GL_VERTEX_ARRAY_BINDING, IntBuffer); int last_vertex_array = IntBuffer[0];
            GL.GetIntegerv(GL.GL_BLEND_SRC, IntBuffer); int last_blend_src = IntBuffer[0];
            GL.GetIntegerv(GL.GL_BLEND_DST, IntBuffer); int last_blend_dst = IntBuffer[0];
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_RGB, IntBuffer); int last_blend_equation_rgb = IntBuffer[0];
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_ALPHA, IntBuffer); int last_blend_equation_alpha = IntBuffer[0];
            GL.GetFloatv(GL.GL_COLOR_CLEAR_VALUE, FloatBuffer);
            float last_clear_color_r = FloatBuffer[0];
            float last_clear_color_g = FloatBuffer[1];
            float last_clear_color_b = FloatBuffer[2];
            float last_clear_color_a = FloatBuffer[3];
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
            GL.Viewport(0, 0, width, height);

            var material = OpenGLMaterial.glyphMaterial;
            var vertexBuffer = textMesh.VertexBuffer;
            var indexBuffer = textMesh.IndexBuffer;

            material.program.Bind();
            material.program.SetUniformMatrix4("ProjMtx", ortho_projection);

            // Send vertex data
            GL.BindVertexArray(material.VaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, material.VboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER, vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), vertexBuffer.Pointer, GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, material.EboHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.Count * Marshal.SizeOf<DrawIndex>(), indexBuffer.Pointer, GL.GL_STREAM_DRAW);

            Utility.CheckGLError();

            GL.Enable(GL.GL_STENCIL_TEST);
            var indexBufferOffset = IntPtr.Zero;
            foreach (var drawCmd in textMesh.Commands)
            {
                var clipRect = drawCmd.ClipRect;
                {
                    // Draw text mesh to stencil buffer
                    GL.StencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_INVERT);
                    GL.StencilFunc(GL.GL_ALWAYS, 1, 1);

                    GL.ColorMask(false, false, false, false);//only draw to stencil buffer
                    GL.Clear(GL.GL_STENCIL_BUFFER_BIT); //clear stencil buffer to 0

                    GL.Scissor((int)clipRect.X, (int)(height - clipRect.Height - clipRect.Y), (int)clipRect.Width, (int)clipRect.Height);
                    GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);

                    Utility.CheckGLError();

                    // Draw text mesh againest stencil buffer
                    GL.StencilFunc(GL.GL_EQUAL, 1, 1);
                    GL.StencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);
                    GL.ColorMask(true, true, true, true);

                    GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);
                }
                indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
            }
            GL.Disable(GL.GL_STENCIL_TEST);

            // Restore modified GL state
            GL.UseProgram((uint)last_program);
            GL.ActiveTexture((uint)last_active_texture);
            GL.BindTexture(GL.GL_TEXTURE_2D, (uint)last_texture);
            GL.BindVertexArray((uint)last_vertex_array);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, (uint)last_array_buffer);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, (uint)last_element_array_buffer);
            GL.BlendEquationSeparate((uint)last_blend_equation_rgb, (uint)last_blend_equation_alpha);
            GL.BlendFunc((uint)last_blend_src, (uint)last_blend_dst);
            if (last_enable_blend == GL.GL_TRUE) GL.Enable(GL.GL_BLEND); else GL.Disable(GL.GL_BLEND);
            if (last_enable_cull_face == GL.GL_TRUE) GL.Enable(GL.GL_CULL_FACE); else GL.Disable(GL.GL_CULL_FACE);
            if (last_enable_depth_test == GL.GL_TRUE) GL.Enable(GL.GL_DEPTH_TEST); else GL.Disable(GL.GL_DEPTH_TEST);
            if (last_enable_scissor_test == GL.GL_TRUE) GL.Enable(GL.GL_SCISSOR_TEST); else GL.Disable(GL.GL_SCISSOR_TEST);
            GL.ClearColor(last_clear_color_r, last_clear_color_g, last_clear_color_b, last_clear_color_a);
            GL.Viewport((int)last_viewport.X, (int)last_viewport.Y, (int)last_viewport.Width, (int)last_viewport.Height);
            GL.Scissor(last_sessor_rect_x, last_sessor_rect_y, last_sessor_rect_width, last_sessor_rect_height);
        }

        public void SwapBuffers()
        {
            eglSwapBuffers(this.display, this.surface);
        }

        public void Unbind()
        {
            //TODO consider Linux usage base on libxcb and GLX
        }

        public void ShutDown()
        {
            OpenGLMaterial.ShutDownCommonMaterials();

            DestroyEGL();
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
