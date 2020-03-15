﻿using CSharpGL;
using ImGui.OSAbstraction.Graphics;
using ImGui.Rendering;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ImGui.OSImplentation.Windows
{
    internal partial class Win32OpenGLRenderer : IRenderer
    {
        public readonly OpenGLMaterial shapeMaterial = new OpenGLMaterial(
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

        public readonly OpenGLMaterial imageMaterial = new OpenGLMaterial(
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

        public static readonly OpenGLMaterial glyphMaterial = new OpenGLMaterial(
    vertexShader: @"
#version 330
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
uniform vec2 offset;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(offset+Position.xy,0,1);
}
",
    fragmentShader: @"
#version 330
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
uniform vec4 color;
void main()
{
	if (Frag_UV.s * Frag_UV.s - Frag_UV.t > 0.0)
	{
		discard;
	}

	// Upper 4 bits: front faces
	// Lower 4 bits: back faces
	Out_Color = Frag_Color* color *0.001 + vec4(0.125, 0, 0, 0); /*Frag_Color* color * (gl_FrontFacing ? 16.0 / 255.0 : 1.0 / 255.0);*/
    Out_Color.a = 1;
}
"
    );
        public static readonly OpenGLMaterial textMaterial = new OpenGLMaterial(
    vertexShader: @"
#version 330
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
    Frag_Color = Color;
	gl_Position = vec4(Position, 0.0, 1.0);
}
",
    fragmentShader: @"
#version 330
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
uniform sampler2D Texture;
void main()
{
	// Get samples for -2/3 and -1/3
	vec2 valueL = texture2D(Texture, vec2(Frag_UV.x + dFdx(Frag_UV.x), Frag_UV.y)).yz * 255.0;
	vec2 lowerL = mod(valueL, 16.0);
	vec2 upperL = (valueL - lowerL) / 16.0;
	vec2 alphaL = min(abs(upperL - lowerL), 2.0);

	// Get samples for 0, +1/3, and +2/3
	vec3 valueR = texture2D(Texture, Frag_UV).xyz * 255.0;
	vec3 lowerR = mod(valueR, 16.0);
	vec3 upperR = (valueR - lowerR) / 16.0;
	vec3 alphaR = min(abs(upperR - lowerR), 2.0);

	// Average the energy over the pixels on either side
	vec4 rgba = vec4(
		(alphaR.x + alphaR.y + alphaR.z) / 6.0,
		(alphaL.y + alphaR.x + alphaR.y) / 6.0,
		(alphaL.x + alphaL.y + alphaR.x) / 6.0,
		0.0);

	Out_Color = vec4(rgba.rgb,1) + Frag_Color*0.001;// + vec4(0,1,0,1);
}
"
    );

        //Helper for some GL functions
        private static readonly int[] IntBuffer = { 0, 0, 0, 0 };
        private static readonly float[] FloatBuffer = { 0, 0, 0, 0 };
        private static readonly uint[] UIntBuffer = { 0, 0, 0, 0 };

        //framebuffer
        private readonly uint[] framebuffers = { 0 };
        private readonly uint[] textures = { 0 };

        static uint framebuffer = 0;
        static uint framebufferColorTexture = 0;
        static QuadMesh quadMesh;
        static ITexture checkerTexture;

        static readonly Point[] JITTER_PATTERN =
        {
            new Point (- 1 / 12.0, -5 / 12.0),
            new Point( 1 / 12.0,  1 / 12.0),
            new Point( 3 / 12.0, -1 / 12.0),
            new Point( 5 / 12.0,  5 / 12.0),
            new Point( 7 / 12.0, -3 / 12.0),
            new Point( 9 / 12.0,  3 / 12.0),
        };
        public void Init(IntPtr windowHandle, Size size)
        {
            CreateOpenGLContext(windowHandle);

            this.shapeMaterial.Init();
            this.imageMaterial.Init();
            glyphMaterial.Init();
            textMaterial.Init();

            // Other state
            GL.Enable(GL.GL_MULTISAMPLE);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.DepthFunc(GL.GL_NEVER);
            GL.Enable(GL.GL_SCISSOR_TEST);

            //set-up framebuffer
            GL.GenFramebuffers(1, framebuffers);
            GL.GenTextures(1, textures);
            framebuffer = framebuffers[0];
            framebufferColorTexture = textures[0];
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, framebuffer);

            //attach color texture to the framebuffer
            GL.BindTexture(GL.GL_TEXTURE_2D, framebufferColorTexture);
            GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA,
                (int)size.Width, (int)size.Height, 0,
                GL.GL_RGB, GL.GL_UNSIGNED_BYTE, IntPtr.Zero);
            GL.FramebufferTexture2D(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT, GL.GL_TEXTURE_2D,
                framebufferColorTexture, 0);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_NEAREST);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_NEAREST);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP);

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

            quadMesh = new QuadMesh(size.Width, size.Height);
            checkerTexture = TextureCache.Default.GetOrAdd(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/checker.jpg");
            Utility.CheckGLError();
        }

        public void Clear(Color clearColor)
        {
            GL.ClearColor((float)clearColor.R, (float)clearColor.G, (float)clearColor.B, (float)clearColor.A);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
        }

        public void DrawMeshes(int width, int height, (Mesh shapeMesh, Mesh imageMesh, TextMesh textMesh) meshes)
        {
            DrawMesh(this.shapeMaterial, meshes.shapeMesh, width, height);
            DrawMesh(this.imageMaterial, meshes.imageMesh, width, height);
            DrawTextMesh(meshes.textMesh, width, height);
        }

        public static void DrawMesh(OpenGLMaterial material, Mesh mesh, int width, int height)
        {
            List<DrawCommand> commandBuffer = mesh.CommandBuffer;
            VertexBuffer vertexBuffer = mesh.VertexBuffer;
            IndexBuffer indexBuffer = mesh.IndexBuffer;

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
            GL.GetIntegerv(GL.GL_SCISSOR_BOX, IntBuffer);
            int last_sessor_rect_x = IntBuffer[0];
            int last_sessor_rect_y = IntBuffer[1];
            int last_sessor_rect_width = IntBuffer[2];
            int last_sessor_rect_height = IntBuffer[3];


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
            GL.Scissor(last_sessor_rect_x, last_sessor_rect_y, last_sessor_rect_width, last_sessor_rect_height);
        }

        /// <summary>
        /// Draw text mesh
        /// </summary>
        internal static void DrawTextMesh(TextMesh textMesh, int width, int height)
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

            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, 0);
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(0, 0, 0, 1);
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
            //for (int j = 0; j < JITTER_PATTERN.Length; j++)
            {
                //var offset = JITTER_PATTERN[j];
                //glyphMaterial.program.SetUniform("offset", offset.x, offset.y);
                //indexBufferOffset = IntPtr.Zero;//reset to redraw with a different offset
                //if(j % 2 == 0)
                //{
                //    glyphMaterial.program.SetUniform("color", j == 0 ? 1 : 0, j == 2 ? 1 : 0, j == 4 ? 1 : 0, 0);
                //}
                foreach (var drawCmd in textMesh.Commands)
                {
                    // Draw text mesh 
                    GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);

                    Utility.CheckGLError();
                    indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
                }
            }
#if false
            //Draw framebuffer texture to screen as a quad,  with the textMaterial applying sub-pixel anti-aliasing
            GL.BindFramebuffer(GL.GL_FRAMEBUFFER_EXT, 0);
            GL.BlendFunc(GL.GL_ZERO, GL.GL_SRC_COLOR);
            textMaterial.program.Bind();
            GL.Disable(GL.GL_SCISSOR_TEST);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.ActiveTexture(GL.GL_TEXTURE0);
            GL.BindTexture(GL.GL_TEXTURE_2D, framebufferColorTexture);
            GL.BindVertexArray(textMaterial.VaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, textMaterial.VboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER, quadMesh.VertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), quadMesh.VertexBuffer.Pointer, GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, textMaterial.EboHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, quadMesh.IndexBuffer.Count * Marshal.SizeOf<DrawIndex>(), quadMesh.IndexBuffer.Pointer, GL.GL_STREAM_DRAW);
            GL.DrawElements(GL.GL_TRIANGLES, quadMesh.CommandBuffer[0].ElemCount, GL.GL_UNSIGNED_INT, IntPtr.Zero);
#endif
            Utility.CheckGLError();

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

        public void ShutDown()
        {
            this.shapeMaterial.ShutDown();
            this.imageMaterial.ShutDown();
            glyphMaterial.ShutDown();
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
