function wasm_exit(exit_code) {
	console.log(`WASM EXIT ${exit_code}`);
}

async function loadRuntime() {
    globalThis.exports = {};
    await import("./dotnet.js");
    return globalThis.exports.createDotnetRuntime;
}

async function main() {
    try {
        const createDotnetRuntime = await loadRuntime();
        const { MONO, BINDING, Module, RuntimeBuildInfo } = await createDotnetRuntime(() => {
            console.log('user code in createDotnetRuntime');
            return {
                disableDotnet6Compatibility: true,
                configSrc: "./mono-config.json",
                preInit: () => { console.log('user code Module.preInit') },
                preRun: () => { console.log('user code Module.preRun') },
                onRuntimeInitialized: () => { console.log('user code Module.onRuntimeInitialized') },
                postRun: () => { console.log('user code Module.postRun') }
            };
        });
        console.log('after createDotnetRuntime');

        // helper for calling gl.bufferData from C#
        window.glBufferDataShim = function (gl, target, bufferPtr, usage, srcOffset, length) {
	        // Create a mapped view over emscripten's Heap.
	        var srcData = new Float32Array(Module.HEAPF32.buffer, bufferPtr, length);
	        console.log('Values heap from glBufferDataShim');
	        for (var i = 0; i < length; i++) {
		        console.log(srcData[i]);
	        } // Call buffer data function
	        gl.bufferData(target, srcData, usage, srcOffset, length);
        };

        const CSharpMain = BINDING.bind_static_method("[WebTemplateApp] WebTemplateApp.Program:Main");
        const ret = CSharpMain();
        wasm_exit(ret);
    } catch (err) {
        console.log(`WASM ERROR ${err}`);
        wasm_exit(2);
    }
}

main();