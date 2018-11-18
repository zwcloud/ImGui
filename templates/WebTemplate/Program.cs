using System;
using System.Runtime.InteropServices;
using WebAssembly;

namespace WebTemplate
{
    public class Program
    {
        static JSObject window;
        static Random random = new System.Random();
        static JSObject randomColorLocation;
        public static void Main(string[] args)
        {
            window = (JSObject)WebAssembly.Runtime.GetGlobalObject("window");
            JSObject document = (JSObject)WebAssembly.Runtime.GetGlobalObject("document");
            JSObject body = (JSObject)document.GetObjectProperty("body");
            JSObject canvas = (JSObject)document.Invoke("createElement", "canvas");
            JSObject canvasStyle = (JSObject)canvas.GetObjectProperty("style");
            canvasStyle.SetObjectProperty("width", "800px");
            canvasStyle.SetObjectProperty("height", "600px");
            body.Invoke("appendChild", canvas);

            JSObject glContext = (JSObject)canvas.Invoke("getContext", "webgl2");
            gl.Init(glContext);

		    gl.clearColor(0.0f, 0.0f, 0.8f, 1.0f);
		    gl.clear(gl.COLOR_BUFFER_BIT);

            var prog = shaderProgram(
                "attribute vec3 pos;"+
                "void main() {"+
                "	gl_Position = vec4(pos, 2.0);"+
                "}",
                "precision mediump float;"+
                "uniform vec3 randomColor;" +
                "void main() {"+
                "	gl_FragColor = vec4(randomColor.r, randomColor.g, randomColor.b, 1.0);"+
                "}"
            );
            gl.useProgram(prog);
            randomColorLocation = gl.getUniformLocation(prog, "randomColor");

            var vertexBufferLength = 3 * 4;
            IntPtr vertexBuffer = Marshal.AllocHGlobal(Marshal.SizeOf<float>()*vertexBufferLength);
            unsafe
            {
                float* p = (float*)vertexBuffer.ToPointer();
                p[0] = -1; p[1] = 0;   p[2] = 0;
                p[3] = 0;  p[4] = 1;   p[5] = 0;
                p[6] = 0;  p[7] = -1;  p[8] = 0;
                p[9] = 1;  p[10] = 0;  p[11] = 0;
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

        static JSObject shaderProgram(string vsText, string fsText) {
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

            if (!gl.getShaderParameter(s, gl.COMPILE_STATUS)) {
                throw new Exception("Could not compile " + type + " shader:\n\n"
                    + gl.getShaderInfoLog(s));
            }

            gl.attachShader(prog, s);
        }

        static void attributeSetFloats(JSObject prog, string attr_name, int size, IntPtr arr, int length) {
            gl.bindBuffer(gl.ARRAY_BUFFER, gl.createBuffer());
            gl.bufferData(gl.ARRAY_BUFFER, arr, gl.STATIC_DRAW, 0, length);
            var attr = gl.getAttribLocation(prog, attr_name);
            gl.enableVertexAttribArray(attr);
            gl.vertexAttribPointer(attr, size, gl.FLOAT, false, 0, 0);
        }

        static void Draw()
        {
            gl.clearColor(0.8f, 0.8f, 0.8f, 1.0f);
            gl.clear(gl.COLOR_BUFFER_BIT);
            gl.uniform3f(randomColorLocation, (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
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
            _gl.Invoke("clearColor", r,g,b,a);
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
}

/*
function shaderProgram(gl, vs, fs) {
	var prog = gl.createProgram();
	var addshader = function(type, source) {
		var s = gl.createShader((type == 'vertex') ? gl.VERTEX_SHADER : gl.FRAGMENT_SHADER);
		gl.shaderSource(s, source);
		gl.compileShader(s);
		gl.getShaderParameter(s, gl.COMPILE_STATUS);
		gl.attachShader(prog, s);
	};
	addshader('vertex', vs);
	addshader('fragment', fs);
	gl.linkProgram(prog);
	gl.getProgramParameter(prog, gl.LINK_STATUS);
	return prog;
}

function attributeSetFloats(gl, prog, attr_name, rsize, arr) {
	gl.bindBuffer(gl.ARRAY_BUFFER, gl.createBuffer());
	gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(arr), gl.STATIC_DRAW);
	var attr = gl.getAttribLocation(prog, attr_name);
	gl.enableVertexAttribArray(attr);
	gl.vertexAttribPointer(attr, rsize, gl.FLOAT, false, 0, 0);
}

function draw() {
	var gl = document.getElementById("webgl").getContext("webgl2");
	gl.clearColor(0.8, 0.8, 0.8, 1);
	gl.clear(gl.COLOR_BUFFER_BIT);

	var prog = shaderProgram(gl,
		"attribute vec3 pos;"+
		"void main() {"+
		"	gl_Position = vec4(pos, 2.0);"+
		"}",
		"void main() {"+
		"	gl_FragColor = vec4(0.5, 0.5, 1.0, 1.0);"+
		"}"
	);
	gl.useProgram(prog);

	attributeSetFloats(gl, prog, "pos", 3, [
		-1, 0, 0,
		0, 1, 0,
		0, -1, 0,
		1, 0, 0
	]);

	gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
}

function init() {
	try {
		draw();
	} catch (e) {
		alert("Error: "+e);
	}
}
setTimeout(init, 100);
 */