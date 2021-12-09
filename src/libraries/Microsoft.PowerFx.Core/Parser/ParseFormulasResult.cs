﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.PowerFx.Core.Errors;
using Microsoft.PowerFx.Core.Syntax.Nodes;
using Microsoft.PowerFx.Core.Utils;
using System.Collections.Generic;

namespace Microsoft.PowerFx.Core.Parser
{
    internal class ParseFormulasResult
    {
        internal Dictionary<DName, TexlNode> NamedFormulas { get; }

        internal List<TexlError> Errors { get; }

        internal bool HasError { get; }

        public ParseFormulasResult(Dictionary<DName, TexlNode> namedFormulas, List<TexlError> errors, bool hasError)
        {
            Contracts.AssertValue(namedFormulas);
            Contracts.Assert(errors == null || hasError);
            NamedFormulas = namedFormulas;
            Errors = errors;
            HasError = hasError;
        }
    }
}