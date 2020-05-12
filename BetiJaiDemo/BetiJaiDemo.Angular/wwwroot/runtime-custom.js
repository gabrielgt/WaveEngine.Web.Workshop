var Module = { 
	onRuntimeInitialized: function () {
		MONO.mono_load_runtime_and_bcl (
			"managed",
			"managed",
			1,
			[ "BetiJaiDemo.dll", "BetiJaiDemo.Web.dll", "Microsoft.Bcl.AsyncInterfaces.dll", "Mono.Security.dll", "mscorlib.dll", "netstandard.dll", "Plugin.Connectivity.Abstractions.dll", "Plugin.Connectivity.dll", "Plugin.DeviceInfo.dll", "System.Core.dll", "System.Data.dll", "System.dll", "System.Drawing.Common.dll", "System.Net.Http.dll", "System.Numerics.dll", "System.Runtime.CompilerServices.Unsafe.dll", "System.Runtime.Serialization.dll", "System.ServiceModel.Internals.dll", "System.Text.Encodings.Web.dll", "System.Text.Json.dll", "System.Xml.dll", "System.Xml.Linq.dll", "WaveEngine.Common.dll", "WaveEngine.Components.dll", "WaveEngine.Framework.dll", "WaveEngine.HLSLEverywhere.dll", "WaveEngine.Mathematics.dll", "WaveEngine.OpenGL.Common.dll", "WaveEngine.Platform.dll", "WaveEngine.Web.dll", "WaveEngine.WebGL.dll", "WaveEngine.Yaml.dll", "WebAssembly.Bindings.dll", "WebAssembly.Net.Http.dll", "WebAssembly.Net.WebSockets.dll", "WebGLDotNET.dll", "Xamarin.Essentials.dll", "BetiJaiDemo.pdb", "BetiJaiDemo.Web.pdb", "Mono.Security.pdb", "mscorlib.pdb", "Plugin.Connectivity.Abstractions.pdb", "Plugin.Connectivity.pdb", "System.Core.pdb", "System.Data.pdb", "System.Drawing.Common.pdb", "System.Net.Http.pdb", "System.Numerics.pdb", "System.pdb", "System.Runtime.Serialization.pdb", "System.ServiceModel.Internals.pdb", "System.Xml.Linq.pdb", "System.Xml.pdb", "WaveEngine.Yaml.pdb", "WebAssembly.Bindings.pdb", "WebAssembly.Net.Http.pdb", "WebAssembly.Net.WebSockets.pdb", "Xamarin.Essentials.pdb" ],
			function () {
				//App.init ();
			}
		);
	},
};


