using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CSharpGL;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSImplentation.Shared;
using SixLabors.ImageSharp.PixelFormats;

namespace ImGui.OSImplentation.Windows
{
    internal partial class Win32OpenGLRenderer
    {
        public byte[] DrawMeshToImage(int width, int height, Mesh mesh, OpenGLMaterial material)
        {
            List<DrawCommand> commandBuffer = mesh.CommandBuffer;
            if (commandBuffer.Count == 0 || commandBuffer[0].ElemCount == 0)
            {
                return null;
            }

            VertexBuffer vertexBuffer = mesh.VertexBuffer;
            IndexBuffer indexBuffer = mesh.IndexBuffer;

            //create texture
            ITexture texture = new OpenGLTexture();
            texture.LoadImage(new Rgba32[width * height], width, height);

            //create frame buffer
            uint[] framebuffers = {0};
            GL.GenFramebuffers(1, framebuffers);
            framebuffer = framebuffers[0];
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, framebuffer);
            GL.BindTexture(GL.GL_TEXTURE_2D, framebufferColorTexture);
            GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT,
                GL.GL_TEXTURE_2D, (uint) texture.GetNativeTextureId(), 0);
            if (GL.CheckFramebufferStatus(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
                throw new Exception("Framebuffer is not complete.");
            }

            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled
            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.DepthFunc(GL.GL_NEVER);
            GL.Enable(GL.GL_SCISSOR_TEST);

            // Setup viewport, orthographic projection matrix
            GL.Viewport(0, 0, width, height);
            float L = 0;
            float R = 0 + width;
            float T = 0;
            float B = 0 + height;
            float[] ortho_projection = new float[16]
            {
                2.0f / (R - L), 0.0f, 0.0f, 0.0f,
                0.0f, 2.0f / (T - B), 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                (R + L) / (L - R), (T + B) / (B - T), 0.0f, 1.0f,
            };
            material.program.Bind();
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

        public byte[] DrawTextMeshToImage(int width, int height, TextMesh mesh,
            OpenGLMaterial material)
        {
            List<DrawCommand> commandBuffer = mesh.Commands;
            if (commandBuffer.Count == 0 || commandBuffer[0].ElemCount == 0)
            {
                return null;
            }

            VertexBuffer vertexBuffer = mesh.VertexBuffer;
            IndexBuffer indexBuffer = mesh.IndexBuffer;

            //create texture
            ITexture texture = new OpenGLTexture();
            texture.LoadImage(new Rgba32[width * height], width, height);

            //create frame buffer
            uint[] framebuffers = {0};
            GL.GenFramebuffers(1, framebuffers);
            framebuffer = framebuffers[0];
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, framebuffer);
            GL.BindTexture(GL.GL_TEXTURE_2D, framebufferColorTexture);
            GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT,
                GL.GL_TEXTURE_2D, (uint) texture.GetNativeTextureId(), 0);
            if (GL.CheckFramebufferStatus(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
                throw new Exception("Framebuffer is not complete.");
            }

            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled
            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.DepthFunc(GL.GL_NEVER);
            GL.Enable(GL.GL_SCISSOR_TEST);

            // Setup viewport, orthographic projection matrix
            GL.Viewport(0, 0, width, height);
            float L = 0;
            float R = 0 + width;
            float T = 0;
            float B = 0 + height;
            float[] ortho_projection = new float[16]
            {
                2.0f / (R - L), 0.0f, 0.0f, 0.0f,
                0.0f, 2.0f / (T - B), 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                (R + L) / (L - R), (T + B) / (B - T), 0.0f, 1.0f,
            };

            material.program.Bind();
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
            foreach (var drawCmd in mesh.Commands)
            {
                // Draw text mesh 
                GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);

                Utility.CheckGLError();
                indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
            }

            //TODO dispose OpenGL resources

            var pixels = new byte[width * height * 4];
            GL.ReadPixels(0, 0, width, height, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
            return pixels;
        }
    }
}