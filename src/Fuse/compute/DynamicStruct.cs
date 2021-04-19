﻿using System;
using System.Collections.Generic;
using System.Text;
using Stride.Core.Extensions;

namespace Fuse.compute
{
    
    public class DynamicStruct: ShaderNode<GpuStruct>
    {
        private readonly int _stride;
        
        public DynamicStruct(IEnumerable<AbstractGpuValue> theInputs, string theName) : base("GPUAttributeStruct")
        {
            Setup(theInputs);
            
            const string shaderCode = 
@"    struct ${structName}{
${structMembers}
    };" ;
            var myStride = 0;
            var call = new StringBuilder();
            Ins.ForEach(input =>
            {
                call.Append("        "+TypeHelpers.GetGpuTypeByValue(input) + " " + input.Name+";"+Environment.NewLine);
                myStride += TypeHelpers.GetSizeInBytes(input);
            });
            Struct = ShaderNodesUtil.Evaluate(shaderCode,new Dictionary<string, string>()
            {
                {"structName", theName},
                {"structMembers", call.ToString()}
            });
            
            Structs = new List<string>(){Struct};
            _stride = myStride;

            Output.TypeOverride = theName;
        }
        
        public sealed override List<string> Structs { get; }
        
        public string Struct { get; }

        protected override string SourceTemplate()
        {
            return "";
        }
        
        public int Stride()
        {
            return _stride;
        }
    }
}