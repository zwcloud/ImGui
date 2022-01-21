using System;
using System.Collections.Generic;

namespace WebAssembly
{
    internal class SR
    {
        public const string ArgumentCannotBeNull = "Invalid argument: {0} can not be null.";
        public const string ArgumentCannotBeNullWithLength = "Invalid argument: {0} can not be null and must have a length";
        public const string CoreObjectErrorBinding = "CoreObject Error binding: {0}";
        public const string ErrorReleasingObject = "Error releasing object {0}";
        public const string HostObjectErrorBinding = "HostObject Error binding: {0}";
        public const string JSObjectErrorBinding = "JSObject Error binding: {0}";
        public const string MultipleHandlesPointingJsId = "Multiple handles pointing at jsId: {0}";
        public const string SystemRuntimeInteropServicesJavaScript_PlatformNotSupported = "System.Private.Runtime.InteropServices.JavaScript is not supported on this platform.";
        public const string TypedArrayNotCorrectType = "TypedArray is not of correct type.";
        public const string UnableCastNullToType = "Unable to cast null to type {0}.";
        public const string UnableCastObjectToType = "Unable to cast object of type {0} to type {1}.";
        public const string ValueTypeNotSupported = "ValueType arguments are not supported.";

        public static string Format(string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
