config = {
 	vfs_prefix: "managed",
 	deploy_prefix: "managed",
 	enable_debugging: 0,
 	file_list: [ "WebTemplateApp.dll","mscorlib.dll","System.Net.Http.dll","System.dll","Mono.Security.dll","System.Xml.dll","System.Core.dll","WebAssembly.Bindings.dll","WebAssembly.Net.Http.dll" ],
	add_bindings: function() { Module.mono_bindings_init ("[WebAssembly.Bindings]WebAssembly.Runtime"); }
}
