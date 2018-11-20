using System;
using System.Runtime.InteropServices;
using WebAssembly;

namespace WebTemplateApp
{
    public class Program
    {
        static JSObject window;
        static Random random = new System.Random();
        static JSObject randomColorLocation;

        private const int canvasWidth = 800;
        private const int canvasHeight = 600;

        public static void Main(string[] args)
        {
            window = (JSObject)WebAssembly.Runtime.GetGlobalObject("window");
            JSObject document = (JSObject)WebAssembly.Runtime.GetGlobalObject("document");
            JSObject body = (JSObject)document.GetObjectProperty("body");
            JSObject canvas = (JSObject)document.Invoke("createElement", "canvas");
            canvas.SetObjectProperty("width", canvasWidth);
            canvas.SetObjectProperty("height", canvasHeight);
            body.Invoke("appendChild", canvas);

            JSObject glContext = (JSObject)canvas.Invoke("getContext", "webgl2");
            gl.Init(glContext);

            gl.clearColor(0.0f, 0.0f, 0.8f, 1.0f);
            gl.clear(gl.COLOR_BUFFER_BIT);

            var prog = shaderProgram(
                "attribute vec3 pos;" +
                "void main() {" +
                "	gl_Position = vec4(pos, 2.0);" +
                "}",
                "precision mediump float;" +
                "uniform vec3 randomColor;" +
                "void main() {" +
                "	gl_FragColor = vec4(randomColor.r, randomColor.g, randomColor.b, 1.0);" +
                "}"
            );
            gl.useProgram(prog);
            randomColorLocation = gl.getUniformLocation(prog, "randomColor");

            var vertexBufferLength = 3 * 4;
            IntPtr vertexBuffer = Marshal.AllocHGlobal(Marshal.SizeOf<float>() * vertexBufferLength);
            unsafe
            {
                float* p = (float*)vertexBuffer.ToPointer();
                p[0] = -1; p[1] = 0; p[2] = 0;
                p[3] = 0; p[4] = 1; p[5] = 0;
                p[6] = 0; p[7] = -1; p[8] = 0;
                p[9] = 1; p[10] = 0; p[11] = 0;
            }

            attributeSetFloats(prog, "pos", 3, vertexBuffer, vertexBufferLength);

            runLoop(0);
        }

        public delegate void Looper(double timeStamp);

        public static Looper l = new Looper(runLoop);

        static void runLoop(double timeStamp)
        {
            window.Invoke("requestAnimationFrame", l);
            Draw();
        }

        static JSObject shaderProgram(string vsText, string fsText)
        {
            var prog = gl.createProgram();
            addshader(prog, "vertex", vsText);
            addshader(prog, "fragment", fsText);
            gl.linkProgram(prog);
            return prog;
        }

        static void addshader(JSObject prog, string type, string source)
        {
            var s = gl.createShader((type == "vertex") ? gl.VERTEX_SHADER : gl.FRAGMENT_SHADER);
            gl.shaderSource(s, source);
            gl.compileShader(s);

            if (!gl.getShaderParameter(s, gl.COMPILE_STATUS))
            {
                throw new Exception("Could not compile " + type + " shader:\n\n"
                    + gl.getShaderInfoLog(s));
            }

            gl.attachShader(prog, s);
        }

        static void attributeSetFloats(JSObject prog, string attr_name, int size, IntPtr arr, int length)
        {
            gl.bindBuffer(gl.ARRAY_BUFFER, gl.createBuffer());
            gl.bufferData(gl.ARRAY_BUFFER, arr, gl.STATIC_DRAW, 0, length);
            var attr = gl.getAttribLocation(prog, attr_name);
            gl.enableVertexAttribArray(attr);
            gl.vertexAttribPointer(attr, size, gl.FLOAT, false, 0, 0);
        }

        static float c = (float)random.NextDouble();
        static void Draw()
        {
            gl.clearColor(0.8f, 0.8f, 0.8f, 1.0f);
            gl.clear(gl.COLOR_BUFFER_BIT);
            Color color = Color.FromHSL((c+=0.003f)%1.0f, 0.5, 0.5);
            gl.uniform3f(randomColorLocation, color.R/255.0f, (float)color.G/255.0f, (float)color.B/255.0f);
            gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
        }
    }

    static class gl
    {
        static JSObject _gl;

        public static void Init(JSObject glContext)
        {
            gl._gl = glContext;
        }

        public static JSObject createProgram()
        {
            return _gl.Invoke("createProgram") as JSObject;
        }

        public static void linkProgram(JSObject prog)
        {
            _gl.Invoke("linkProgram", prog);
        }

        public const int VERTEX_SHADER = 35633;
        public const int FRAGMENT_SHADER = 35632;

        public static JSObject createShader(int type)
        {
            return _gl.Invoke("createShader", type) as JSObject;
        }

        public static void shaderSource(JSObject shaderObject, string source)
        {
            _gl.Invoke("shaderSource", shaderObject, source);
        }

        public static void compileShader(JSObject shaderObject)
        {
            _gl.Invoke("compileShader", shaderObject);
        }

        public static void attachShader(JSObject program, JSObject shaderObject)
        {
            _gl.Invoke("attachShader", program, shaderObject);
        }

        public static JSObject createBuffer()
        {
            return _gl.Invoke("createBuffer") as JSObject;
        }

        public const int ARRAY_BUFFER = 34962;

        public static void bindBuffer(int target, JSObject buffer)
        {
            _gl.Invoke("bindBuffer", target, buffer);
        }

        public const int STATIC_DRAW = 35044;

        public static void bufferData(int target, IntPtr srcData, int usage, int srcOffset, int length)
        {
            JSObject window = WebAssembly.Runtime.GetGlobalObject("window") as JSObject;
            window.Invoke("glBufferDataShim", _gl, target, srcData.ToInt32(), usage, srcOffset, length);
        }

        public static int getAttribLocation(JSObject prog, string attr_name)
        {
            return (int)_gl.Invoke("getAttribLocation", prog, attr_name);
        }

        public static void enableVertexAttribArray(int attribute)
        {
            _gl.Invoke("enableVertexAttribArray", attribute);
        }

        public const int FLOAT = 5126;

        public static void vertexAttribPointer(int attribute, int size, int type, bool normalized, int stride, int offset)
        {
            _gl.Invoke("vertexAttribPointer", attribute, size, type, normalized, stride, offset);
        }

        public static void clearColor(float r, float g, float b, float a)
        {
            _gl.Invoke("clearColor", r, g, b, a);
        }

        public const int COLOR_BUFFER_BIT = 16384;

        public static void clear(int mask)
        {
            _gl.Invoke("clear", mask);
        }

        public static void useProgram(JSObject prog)
        {
            _gl.Invoke("useProgram", prog);
        }

        public const int TRIANGLE_STRIP = 5;

        public static void drawArrays(int mode, int first, int size)
        {
            _gl.Invoke("drawArrays", mode, first, size);
        }

        public static JSObject getUniformLocation(JSObject program, string name)
        {
            return _gl.Invoke("getUniformLocation", program, name) as JSObject;
        }

        public static void uniform3f(JSObject location, float value0, float value1, float value2)
        {
            _gl.Invoke("uniform3f", location, value0, value1, value2);
        }

        public const int COMPILE_STATUS = 35713;

        public static bool getShaderParameter(JSObject shader, int pname)
        {
            return (bool)_gl.Invoke("getShaderParameter", shader, pname);
        }

        public static string getShaderInfoLog(JSObject shader)
        {
            return _gl.Invoke("getShaderInfoLog", shader) as string;
        }
    }

    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;

        public Color(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static Color FromHSL(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                    r = v;
                    g = mid1;
                    b = m;
                    break;
                    case 1:
                    r = mid2;
                    g = v;
                    b = m;
                    break;
                    case 2:
                    r = m;
                    g = v;
                    b = mid1;
                    break;
                    case 3:
                    r = m;
                    g = mid2;
                    b = v;
                    break;
                    case 4:
                    r = mid1;
                    g = m;
                    b = v;
                    break;
                    case 5:
                    r = v;
                    g = m;
                    b = mid2;
                    break;
                }
            }
            Color rgb;
            rgb.R = Convert.ToByte(r * 255.0f);
            rgb.G = Convert.ToByte(g * 255.0f);
            rgb.B = Convert.ToByte(b * 255.0f);
            return rgb;
        }
    }

}
