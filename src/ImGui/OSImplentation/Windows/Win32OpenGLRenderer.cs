using System;
using System.Collections.Generic;
using CSharpGL;
using System.Runtime.InteropServices;
using ImGui.Common;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal partial class Win32OpenGLRenderer : IRenderer
    {
        private readonly OpenGLMaterial shapeMaterial = new OpenGLMaterial(
            vertexShader: @"
#version 330
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(Position.xy,0,1);
}
",
            fragmentShader: @"
#version 330
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	Out_Color = Frag_Color;
}
"
            );

        private readonly OpenGLMaterial imageMaterial = new OpenGLMaterial(
            vertexShader: @"
#version 330
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(Position.xy,0,1);
}
",
            fragmentShader: @"
#version 330
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	Out_Color = Frag_Color * texture( Texture, Frag_UV.st);
}
"
            );

        private readonly OpenGLMaterial glyphMaterial = new OpenGLMaterial(
    vertexShader: @"
#version 330
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(Position.xy,0,1);
}
",
    fragmentShader: @"
#version 330
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	if (Frag_UV.s * Frag_UV.s - Frag_UV.t > 0.0)
	{
		discard;
	}
	Out_Color = Frag_Color;
}
"
    );
        //Helper for some GL functions
        private static readonly int[] IntBuffer = { 0, 0, 0, 0 };
        private static readonly float[] FloatBuffer = { 0, 0, 0, 0 };
        private static readonly uint[] UIntBuffer = { 0, 0, 0, 0 };

        //#START
        private uint renderedTexture;
        private uint textFrameBuffer;
        private readonly UnsafeList<DrawVertex> quadVertices = new UnsafeList<DrawVertex>(4);
        private readonly UnsafeList<DrawIndex> quadIndices = new UnsafeList<DrawIndex>(6);
        //#END

        public void Init(IntPtr windowHandle, Size size)
        {
            CreateOpenGLContext((IntPtr)windowHandle);

            this.shapeMaterial.Init();
            this.imageMaterial.Init();
            this.glyphMaterial.Init();
            {
                this.quadVertices.Add(new DrawVertex { pos = (0, 0), uv = (0, 0), color = (ColorF)Color.Black });
                this.quadVertices.Add(new DrawVertex { pos = (0, 0), uv = (0, 1), color = (ColorF)Color.Black });
                this.quadVertices.Add(new DrawVertex { pos = (0, 0), uv = (1, 1), color = (ColorF)Color.Black });
                this.quadVertices.Add(new DrawVertex { pos = (0, 0), uv = (1, 0), color = (ColorF)Color.Black });

                this.quadIndices.Add(new DrawIndex { Index = 0 });
                this.quadIndices.Add(new DrawIndex { Index = 1 });
                this.quadIndices.Add(new DrawIndex { Index = 2 });
                this.quadIndices.Add(new DrawIndex { Index = 2 });
                this.quadIndices.Add(new DrawIndex { Index = 3 });
                this.quadIndices.Add(new DrawIndex { Index = 0 });
            }

            // render target
            {
                GL.GenFramebuffersEXT(1, UIntBuffer);
                this.textFrameBuffer = UIntBuffer[0];
                GL.BindFramebufferEXT(GL.GL_FRAMEBUFFER_EXT, this.textFrameBuffer);

                //create the texture which will contain the RGB output of our shader.
                // The texture we're going to render to
                GL.GenTextures(1, UIntBuffer);
                this.renderedTexture = UIntBuffer[0];

                // "Bind" the newly created texture : all future texture functions will modify this texture
                GL.BindTexture(GL.GL_TEXTURE_2D, this.renderedTexture);

                // Give an empty image to OpenGL ( the last "0" )
                GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, (int)size.Width, (int)size.Height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, IntPtr.Zero);

                // Poor filtering. Needed !
                GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_NEAREST);
                GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_NEAREST);

                // configure our framebuffer
                // Set "renderedTexture" as our colour attachement #0
                GL.FramebufferTexture(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT, this.renderedTexture, 0);

                // Set the list of draw buffers.
                UIntBuffer[0] = GL.GL_COLOR_ATTACHMENT0_EXT;//DrawBuffers
                GL.DrawBuffers(1, UIntBuffer); // "1" is the size of DrawBuffers

                // check that our framebuffer is ok
                if (GL.CheckFramebufferStatusEXT(GL.GL_FRAMEBUFFER_EXT) != GL.GL_FRAMEBUFFER_COMPLETE_EXT)
                {
                    throw new Exception("Failed to create framebuffer.");
                }

                // restore to default framebuffer
                GL.BindFramebufferEXT(GL.GL_FRAMEBUFFER_EXT, 0);
                // unbind texture
                GL.BindTexture(GL.GL_TEXTURE_2D, 0);
            }

            // Other state
            GL.Enable(GL.GL_MULTISAMPLE);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.DepthFunc(GL.GL_NEVER);
            GL.Enable(GL.GL_SCISSOR_TEST);
            var clearColor = Color.Argb(255, 114, 144, 154);//TODO this should be the background color of Form
            GL.ClearColor((float)clearColor.R, (float)clearColor.G, (float)clearColor.B, (float)clearColor.A);

            Utility.CheckGLError();
        }

        public void Clear()
        {
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
        }

        public void RenderDrawList(DrawList drawList, int width, int height)
        {
            DrawMesh(this.shapeMaterial, drawList.ShapeMesh, width, height);
            DrawMesh(this.imageMaterial, drawList.ImageMesh, width, height);

            DrawTextMesh(drawList.TextMesh, width, height);
        }

        private static void DrawMesh(OpenGLMaterial material, Mesh mesh, int width, int height)
        {
            List<DrawCommand> commandBuffer = mesh.CommandBuffer;
            UnsafeList<DrawVertex> vertexBuffer = mesh.VertexBuffer;
            UnsafeList<DrawIndex> indexBuffer = mesh.IndexBuffer;

            // Backup GL state
            GL.GetIntegerv(GL.GL_CURRENT_PROGRAM, IntBuffer); int last_program = IntBuffer[0];
            GL.GetIntegerv(GL.GL_TEXTURE_BINDING_2D, IntBuffer); int last_texture = IntBuffer[0];
            GL.GetIntegerv(GL.GL_ACTIVE_TEXTURE, IntBuffer); int last_active_texture = IntBuffer[0];
            GL.GetIntegerv(GL.GL_ARRAY_BUFFER_BINDING, IntBuffer); int last_array_buffer = IntBuffer[0];
            GL.GetIntegerv(GL.GL_ELEMENT_ARRAY_BUFFER_BINDING, IntBuffer); int last_element_array_buffer = IntBuffer[0];
            GL.GetIntegerv(GL.GL_VERTEX_ARRAY_BINDING, IntBuffer);int last_vertex_array = IntBuffer[0];
            GL.GetIntegerv(GL.GL_BLEND_SRC, IntBuffer); int last_blend_src = IntBuffer[0];
            GL.GetIntegerv(GL.GL_BLEND_DST, IntBuffer); int last_blend_dst = IntBuffer[0];
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_RGB, IntBuffer); int last_blend_equation_rgb = IntBuffer[0];
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_ALPHA, IntBuffer);int last_blend_equation_alpha = IntBuffer[0];
            GL.GetIntegerv(GL.GL_VIEWPORT, IntBuffer); Rect last_viewport = new Rect(IntBuffer[0], IntBuffer[1], IntBuffer[2], IntBuffer[3]);
            uint last_enable_blend = GL.IsEnabled(GL.GL_BLEND);
            uint last_enable_cull_face = GL.IsEnabled(GL.GL_CULL_FACE);
            uint last_enable_depth_test = GL.IsEnabled(GL.GL_DEPTH_TEST);
            uint last_enable_scissor_test = GL.IsEnabled(GL.GL_SCISSOR_TEST);

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
            GLM.mat4 ortho_projection = GLM.glm.ortho(0.0f, width, height, 0.0f, -5.0f, 5.0f);
            material.program.Bind();
            material.program.SetUniformMatrix4("ProjMtx", ortho_projection.to_array());//FIXME make GLM.mat4.to_array() not create a new array

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
            GL.BlendFunc((uint)last_blend_src, (uint)last_blend_dst);
            if (last_enable_blend == GL.GL_TRUE) GL.Enable(GL.GL_BLEND); else GL.Disable(GL.GL_BLEND);
            if (last_enable_cull_face == GL.GL_TRUE) GL.Enable(GL.GL_CULL_FACE); else GL.Disable(GL.GL_CULL_FACE);
            if (last_enable_depth_test == GL.GL_TRUE) GL.Enable(GL.GL_DEPTH_TEST); else GL.Disable(GL.GL_DEPTH_TEST);
            if (last_enable_scissor_test == GL.GL_TRUE) GL.Enable(GL.GL_SCISSOR_TEST); else GL.Disable(GL.GL_SCISSOR_TEST);
            GL.Viewport((int)last_viewport.X, (int)last_viewport.Y, (int)last_viewport.Width, (int)last_viewport.Height);
        }

        /// <summary>
        /// Draw text mesh
        /// </summary>
        private void DrawTextMesh(TextMesh textMesh, int width, int height)
        {
            // Backup GL state
            GL.GetIntegerv(GL.GL_FRAMEBUFFER_BINDING_EXT, IntBuffer); int last_framebuffer_binding = IntBuffer[0];
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

            GLM.mat4 ortho_projection = GLM.glm.ortho(0.0f, width, height, 0.0f, -5.0f, 5.0f);
            GL.Viewport(0, 0, width, height);

            GL.Enable(GL.GL_STENCIL_TEST);
            GL.StencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_INVERT);
            GL.StencilFunc(GL.GL_ALWAYS, 1, 1);

            GL.ColorMask(false, false, false, false);//only draw to stencil buffer
            GL.Clear(GL.GL_STENCIL_BUFFER_BIT); //clear stencil buffer to 0

            Utility.CheckGLError();

            {
                // Draw text mesh to stencil buffer
                var material = this.glyphMaterial;
                var vertexBuffer = textMesh.VertexBuffer;
                var indexBuffer = textMesh.IndexBuffer;

                material.program.Bind();
                material.program.SetUniformMatrix4("ProjMtx", ortho_projection.to_array());//FIXME make GLM.mat4.to_array() not create a new array

                // Send vertex data
                GL.BindVertexArray(material.VaoHandle);
                GL.BindBuffer(GL.GL_ARRAY_BUFFER, material.VboHandle);
                GL.BufferData(GL.GL_ARRAY_BUFFER, vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), vertexBuffer.Pointer, GL.GL_STREAM_DRAW);
                GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, material.EboHandle);
                GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.Count * Marshal.SizeOf<DrawIndex>(), indexBuffer.Pointer, GL.GL_STREAM_DRAW);

                var drawCmd = textMesh.Command;
                var clipRect = drawCmd.ClipRect;
                GL.Scissor((int)clipRect.X, (int)(height - clipRect.Height - clipRect.Y), (int)clipRect.Width, (int)clipRect.Height);
                GL.DrawElements(GL.GL_TRIANGLES, indexBuffer.Count, GL.GL_UNSIGNED_INT, IntPtr.Zero);

                Utility.CheckGLError();

                GL.StencilFunc(GL.GL_EQUAL, 1, 1);
                GL.StencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);
                GL.ColorMask(true, true, true, true);
                // Draw text mesh againest stencil buffer
                GL.DrawElements(GL.GL_TRIANGLES, indexBuffer.Count, GL.GL_UNSIGNED_INT, IntPtr.Zero);
            }
            GL.Disable(GL.GL_STENCIL_TEST);

            // Restore modified GL state
            GL.BindFramebufferEXT(GL.GL_FRAMEBUFFER_EXT, (uint)last_framebuffer_binding);
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
        }

        public void ShutDown()
        {
            this.shapeMaterial.ShutDown();
            this.imageMaterial.ShutDown();
            this.glyphMaterial.ShutDown();

            //TODO release frame buffer
        }
    }
}
