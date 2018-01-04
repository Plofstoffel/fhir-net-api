﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Specification.Schema.Tags;
using Hl7.Fhir.Utility;

namespace Hl7.Fhir.Specification.Schema
{
    public class ConditionalAssertion : Assertion, IGroupAssertion
    {
        public readonly ElementSchema Condition;

        public readonly ElementSchema Assertion;

        public ConditionalAssertion(ElementSchema condition, ElementSchema assertion)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            Assertion = assertion ?? throw new ArgumentNullException(nameof(assertion));
        }

        public override IEnumerable<SchemaTags> CollectTags() => Assertion.CollectTags();

        public SchemaTags Validate(IEnumerable<IElementNavigator> input, ValidationContext vc)
        {
            var result = Condition.Validate(input,vc);

            switch(result.Result.Result)
            {
                case ValidationResult.Success:
                    // is a member, result depends on membership assertion
                    return result + Assertion.Validate(input, vc);  
                case ValidationResult.Failure:
                    // fails membership condition -> no reason to fail
                    return SchemaTags.Success;
                case ValidationResult.Undecided:
                    return SchemaTags.Undecided;
                default:
                    // TODO: add context information (location + assertion ID) for debug purposes
                    throw Error.NotSupported($"Internal error: Unknown validation result '{result.Result.Result}' encountered in group.");
            }
        }
    }

}
