/*
   Copyright 2017-2023 Adrian Popescu.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
       http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Padi.Vies.Validators;

[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
internal sealed class HrVatValidator : VatValidatorAbstract
{
    private const string REGEX_PATTERN =@"^\d{11}$";
    private const string COUNTRY_CODE = nameof(EuCountryCode.HR);

    private static readonly Regex _regex = new(REGEX_PATTERN, RegexOptions.Compiled, TimeSpan.FromSeconds(5));

    public HrVatValidator()
    {
        this.Regex = _regex;
        CountryCode = COUNTRY_CODE;
    }
        
    protected override VatValidationResult OnValidate(string vat)
    {
        var product = 10;

        for (var index = 0; index < 10; index++)
        {
            var sum = (vat[index].ToInt() + product) % 10;
               
            if (sum == 0)
            {
                sum = 10;
            }
                
            product = 2 * sum % 11;
        }

        var checkDigit = (product + vat[10].ToInt()) % 10;
            
        var isValid = checkDigit == 1;
        return !isValid 
            ? VatValidationResult.Failed("Invalid HR vat: checkValue") 
            : VatValidationResult.Success();
    }
}