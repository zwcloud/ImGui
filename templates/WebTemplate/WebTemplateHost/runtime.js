var Module = {
	onRuntimeInitialized: function () {
		MONO.mono_load_runtime_and_bcl (
		config.vfs_prefix,
		config.deploy_prefix,
		config.enable_debugging,
		config.file_list,
		function () {
			config.add_bindings ();
			//call main()
			BINDING.call_static_method("[WebTemplateApp] WebTemplateApp.Program:Main", []);
		}
	)
	},
};

// helper for calling gl.bufferData from C#
var glBufferDataShim = function (gl, target, bufferPtr, usage, srcOffset, length) {// Create a mapped view over emscripten's Heap.
    var srcData = new Float32Array(Module.HEAPF32.buffer, bufferPtr, length );
    console.log('Values heap from glBufferDataShim');
    for (var i = 0; i< length; i++)
    {
         console.log(srcData[i]);
    } // Call buffer data function
    gl.bufferData(target, srcData, usage, srcOffset, length);
}