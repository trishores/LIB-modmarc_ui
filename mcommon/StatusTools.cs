/*
Copyright (C) 2020-2021 Tris Shores

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Text.RegularExpressions;

namespace marc_common
{
    internal static class c_StatusTools
    {
        internal static string m_GetErrorText(c_StatusCode v_error)
        {
            var v_enumStr = Enum.GetName(typeof(c_StatusCode), (int)v_error);
            var v_readableStr = Regex.Replace(v_enumStr, "([a-z0-9]{1})([A-Z]{1})", v_m => $"{v_m.Groups[1].Value} {v_m.Groups[2].Value.ToLower()}");
            return v_readableStr;
        }

        internal static string m_GetErrorText(int v_exitCode)
        {
            var v_enumStr = Enum.GetName(typeof(c_StatusCode), v_exitCode);
            if (v_enumStr == null) return null;
            var v_readableStr = Regex.Replace(v_enumStr, "([a-z0-9]{1})([A-Z]{1})", v_m => $"{v_m.Groups[1].Value} {v_m.Groups[2].Value.ToLower()}");
            return v_readableStr;
        }

        internal enum c_StatusCode
        {
            Ok = 0,
            UsernameIsBlank = 1,
            PasswordIsBlank = 2,
            BookFileNotFound = 3,
            LicenseFileNotFound = 4,
            InvalidLicenseKey = 5,
            ExpiredApplicationLicense = 6,
            PremiumLicenseHasExpired = 7,
            ComputerTimeAnomalyDetected = 8,
            InvalidCredentials = 9,
            EngineComponentNotFound = 10,
            UnknownError = 11,
            MetricComponentNotFound = 12,
            InvalidMetricJson = 13,
            MarcFileNotFound = 14,
            MarcFileParseError = 15,
            DatFileNotFound = 16,
            UnsupportedMarcFile = 17,
            LicenseForWrongApp = 18,
            InvalidNumberOfEngineArguments = 19,
            InvalidPipeId = 20,
            EngineActionNotSpecified = 21,
            PipeError = 22,
            CacheFileNotFound = 23,
            ServerUnreachable = 24,
            InvalidOperation = 25,
        }

        internal enum c_EngineActionCode
        {
            ValidateOnly = 0,
            GenerateMarc = 1,
            DatToMrc = 2,
            MrcToDat = 3,
            GetAzureFunctionBaseUrl = 4,
            GetDecryptedCache = 5,
            SetEncryptedCache = 6,
        }
    }
}