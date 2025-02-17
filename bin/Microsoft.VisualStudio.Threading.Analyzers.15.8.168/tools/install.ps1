﻿param($installPath, $toolsPath, $package, $project)

$analyzersPaths = Join-Path (Join-Path (Split-Path -Path $toolsPath -Parent) "analyzers") * -Resolve

foreach($analyzersPath in $analyzersPaths)
{
    # Install the language agnostic analyzers.
    if (Test-Path $analyzersPath)
    {
        foreach ($analyzerFilePath in Get-ChildItem -Path "$analyzersPath\*.dll" -Exclude *.resources.dll)
        {
            if($project.Object.AnalyzerReferences)
            {
                $project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
            }
        }
    }
}

# $project.Type gives the language name like (C# or VB.NET)
$languageFolder = ""
if($project.Type -eq "C#")
{
    $languageFolder = "cs"
}
if($project.Type -eq "VB.NET")
{
    $languageFolder = "vb"
}
if($languageFolder -eq "")
{
    return
}

foreach($analyzersPath in $analyzersPaths)
{
    # Install language specific analyzers.
    $languageAnalyzersPath = join-path $analyzersPath $languageFolder
    if (Test-Path $languageAnalyzersPath)
    {
        foreach ($analyzerFilePath in Get-ChildItem -Path "$languageAnalyzersPath\*.dll" -Exclude *.resources.dll)
        {
            if($project.Object.AnalyzerReferences)
            {
                $project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
            }
        }
    }
}
# SIG # Begin signature block
# MIIapwYJKoZIhvcNAQcCoIIamDCCGpQCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQU+a8Yw7Lo6SPSbHJD3x4tGJTH
# 5LCgghWIMIIE2jCCA8KgAwIBAgITMwAAARGDUctkxt4WEAAAAAABETANBgkqhkiG
# 9w0BAQUFADB3MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4G
# A1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSEw
# HwYDVQQDExhNaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EwHhcNMTgwODIzMjAyMDQy
# WhcNMTkxMTIzMjAyMDQyWjCByjELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hp
# bmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jw
# b3JhdGlvbjElMCMGA1UECxMcTWljcm9zb2Z0IEFtZXJpY2EgT3BlcmF0aW9uczEm
# MCQGA1UECxMdVGhhbGVzIFRTUyBFU046NTdDOC0yRDE1LTFDOEIxJTAjBgNVBAMT
# HE1pY3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZpY2UwggEiMA0GCSqGSIb3DQEBAQUA
# A4IBDwAwggEKAoIBAQCfbGtk7EDdi7vg7CjxNGsIhu9UjX85htiGv99P/KMd+858
# 2d2tInDLXkGBBsCMm+EFFOOEonHgRuo2w0SKQR76uTAAaP54jCrs4kTFAZd7UJrn
# JrCuIQzRytKJNHMaGFBPjaCYsI3MCP2r7ur4p1L9TXbYosTJnBu7Akc/JGOzB9/3
# 8vnh6yRExEOo7hjP1w2XiBrvsSLij+zxnnJuPt6Z5JF78isLPpU3NMBOFzUagFq6
# SwaFwDCODqX0753olXRM/FrNFqGATPx5dk5SVLmUOS8YVR8hVM4ZivXR3jV2/Zdf
# Pb9gcfNUikkgdD1iNYVtRYumcH50RWfVicNXyhcnAgMBAAGjggEJMIIBBTAdBgNV
# HQ4EFgQUhkuSCfqstP/Hva/Oqi24ToeBwBwwHwYDVR0jBBgwFoAUIzT42VJGcArt
# QPt2+7MrsMM1sw8wVAYDVR0fBE0wSzBJoEegRYZDaHR0cDovL2NybC5taWNyb3Nv
# ZnQuY29tL3BraS9jcmwvcHJvZHVjdHMvTWljcm9zb2Z0VGltZVN0YW1wUENBLmNy
# bDBYBggrBgEFBQcBAQRMMEowSAYIKwYBBQUHMAKGPGh0dHA6Ly93d3cubWljcm9z
# b2Z0LmNvbS9wa2kvY2VydHMvTWljcm9zb2Z0VGltZVN0YW1wUENBLmNydDATBgNV
# HSUEDDAKBggrBgEFBQcDCDANBgkqhkiG9w0BAQUFAAOCAQEAL5YjkjdYxGvQVeph
# Rug+5YRXkM4TBrecNtPxFqpFDqu8aALUp9v/j4tCJ+4Mwyhsf65rVwZYljE0zfzK
# eOMZVf2XyBdhBWaECWtN8Ga1Br67qUPY3i9cVqRm3dxLowH4cy5WlbBbzrr6IozG
# MmCSDEejkrY703YapocSy96bLWYbQ2j6NiWG9LE71FnADlAbdKGyDMRGrDwtEiVa
# lFLdnCdeYO8DhpOPYkCtW3vi0da3jkZDPQ1r5seZR0XQHz9K+qY7y37uZOZMZe/V
# TKs9O9Qq+wB8JGm0LHfoobwPXVO1z4G3+Y9b1tjLhFQHmdALBIByLyxg9PfGBplZ
# RDHEXTCCBNswggPDoAMCAQICEzMAAAGx3e26VOlluF8AAQAAAbEwDQYJKoZIhvcN
# AQEFBQAweTELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNV
# BAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEjMCEG
# A1UEAxMaTWljcm9zb2Z0IENvZGUgU2lnbmluZyBQQ0EwHhcNMTgwNzEyMjAxMTE5
# WhcNMTkwNzI2MjAxMTE5WjB0MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGlu
# Z3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBv
# cmF0aW9uMR4wHAYDVQQDExVNaWNyb3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqG
# SIb3DQEBAQUAA4IBDwAwggEKAoIBAQCbUr5PLLQdU8s9XSMetoOjn8DlSuBXs8L4
# LdaKkV/MEs4M3QK3Rnj0a5ve58sgIY/UPg4llHI6eGNmLrVf4duX/YiOqIiXVUwd
# Hj6paUktE2P5fsOlqtO/f8IHOSEwZjaVl2+n3qulEvaF6WgoaSabGp63r+xTeG/D
# zDEwh/b3NiswQFlIRoBRqM0MgN3jSp4tdFXs5qdEetUTvvTCJH0M/TPN5iNfms3U
# KW0C1TJaaifPsMkBi5Sv5QyLeh99IhODhvJaS9OEY1caa0l4OlTpj+GFxuU/liXn
# XvnIy8eh8vIbDSJbuS/8jOnbKMRVF7bo9Nd6z/2c04/u6XYqgoyhAgMBAAGjggFf
# MIIBWzATBgNVHSUEDDAKBggrBgEFBQcDAzAdBgNVHQ4EFgQURBO0Ap13PpBVcfXk
# HH+sFxTPPnMwUAYDVR0RBEkwR6RFMEMxKTAnBgNVBAsTIE1pY3Jvc29mdCBPcGVy
# YXRpb25zIFB1ZXJ0byBSaWNvMRYwFAYDVQQFEw0yMjk4MDMrNDM3OTUwMB8GA1Ud
# IwQYMBaAFMsR6MrStBZYAck3LjMWFrlMmgofMFYGA1UdHwRPME0wS6BJoEeGRWh0
# dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY0NvZFNp
# Z1BDQV8wOC0zMS0yMDEwLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKG
# Pmh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljQ29kU2lnUENB
# XzA4LTMxLTIwMTAuY3J0MA0GCSqGSIb3DQEBBQUAA4IBAQCDqfKmM8WhexzSXoCu
# 7KSmw+koXVclI43dJQlpcOhNaRYvlLsF2ZuA7FnQaGQbgzSo3M9jjZ1rdgD1e69C
# dAfF/JHA4qxFM5Zdsf36jkaySeJDBhmYhXF4057p25UWQTE0cCxDlqICd6kvPWFK
# Y1nxl33RUpVghai8G0UHP9nBj/dQ9bl+CInkQsRYjkHAlThJH9cd+DBPKS87R1mS
# N+6XIe0XZpAcLy1ta/8LdeWqEt36r8QRtOAhMlq/Bc5F+T+NVq7OUdbMrn0EGTAS
# 8r+fSCacDgANKK2l3kQtzNSH/5RJlVcMTTScIwrdw40yCfS9NSjF3tCIP6kSE6Ew
# cKARMIIFvDCCA6SgAwIBAgIKYTMmGgAAAAAAMTANBgkqhkiG9w0BAQUFADBfMRMw
# EQYKCZImiZPyLGQBGRYDY29tMRkwFwYKCZImiZPyLGQBGRYJbWljcm9zb2Z0MS0w
# KwYDVQQDEyRNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkwHhcN
# MTAwODMxMjIxOTMyWhcNMjAwODMxMjIyOTMyWjB5MQswCQYDVQQGEwJVUzETMBEG
# A1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWlj
# cm9zb2Z0IENvcnBvcmF0aW9uMSMwIQYDVQQDExpNaWNyb3NvZnQgQ29kZSBTaWdu
# aW5nIFBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALJyWVwZMGS/
# HZpgICBCmXZTbD4b1m/My/Hqa/6XFhDg3zp0gxq3L6Ay7P/ewkJOI9VyANs1VwqJ
# yq4gSfTwaKxNS42lvXlLcZtHB9r9Jd+ddYjPqnNEf9eB2/O98jakyVxF3K+tPeAo
# aJcap6Vyc1bxF5Tk/TWUcqDWdl8ed0WDhTgW0HNbBbpnUo2lsmkv2hkL/pJ0KeJ2
# L1TdFDBZ+NKNYv3LyV9GMVC5JxPkQDDPcikQKCLHN049oDI9kM2hOAaFXE5Wgigq
# BTK3S9dPY+fSLWLxRT3nrAgA9kahntFbjCZT6HqqSvJGzzc8OJ60d1ylF56NyxGP
# VjzBrAlfA9MCAwEAAaOCAV4wggFaMA8GA1UdEwEB/wQFMAMBAf8wHQYDVR0OBBYE
# FMsR6MrStBZYAck3LjMWFrlMmgofMAsGA1UdDwQEAwIBhjASBgkrBgEEAYI3FQEE
# BQIDAQABMCMGCSsGAQQBgjcVAgQWBBT90TFO0yaKleGYYDuoMW+mPLzYLTAZBgkr
# BgEEAYI3FAIEDB4KAFMAdQBiAEMAQTAfBgNVHSMEGDAWgBQOrIJgQFYnl+UlE/wq
# 4QpTlVnkpDBQBgNVHR8ESTBHMEWgQ6BBhj9odHRwOi8vY3JsLm1pY3Jvc29mdC5j
# b20vcGtpL2NybC9wcm9kdWN0cy9taWNyb3NvZnRyb290Y2VydC5jcmwwVAYIKwYB
# BQUHAQEESDBGMEQGCCsGAQUFBzAChjhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20v
# cGtpL2NlcnRzL01pY3Jvc29mdFJvb3RDZXJ0LmNydDANBgkqhkiG9w0BAQUFAAOC
# AgEAWTk+fyZGr+tvQLEytWrrDi9uqEn361917Uw7LddDrQv+y+ktMaMjzHxQmIAh
# Xaw9L0y6oqhWnONwu7i0+Hm1SXL3PupBf8rhDBdpy6WcIC36C1DEVs0t40rSvHDn
# qA2iA6VW4LiKS1fylUKc8fPv7uOGHzQ8uFaa8FMjhSqkghyT4pQHHfLiTviMocro
# E6WRTsgb0o9ylSpxbZsa+BzwU9ZnzCL/XB3Nooy9J7J5Y1ZEolHN+emjWFbdmwJF
# RC9f9Nqu1IIybvyklRPk62nnqaIsvsgrEA5ljpnb9aL6EiYJZTiU8XofSrvR4Vbo
# 0HiWGFzJNRZf3ZMdSY4tvq00RBzuEBUaAF3dNVshzpjHCe6FDoxPbQ4TTj18KUic
# ctHzbMrB7HCjV5JXfZSNoBtIA1r3z6NnCnSlNu0tLxfI5nI3EvRvsTxngvlSso0z
# FmUeDordEN5k9G/ORtTTF+l5xAS00/ss3x+KnqwK+xMnQK3k+eGpf0a7B2BHZWBA
# TrBC7E7ts3Z52Ao0CW0cgDEf4g5U3eWh++VHEK1kmP9QFi58vwUheuKVQSdpw5OP
# lcmN2Jshrg1cnPCiroZogwxqLbt2awAdlq3yFnv2FoMkuYjPaqhHMS+a3ONxPdcA
# fmJH0c6IybgY+g5yjcGjPa8CQGr/aZuW4hCoELQ3UAjWwz0wggYHMIID76ADAgEC
# AgphFmg0AAAAAAAcMA0GCSqGSIb3DQEBBQUAMF8xEzARBgoJkiaJk/IsZAEZFgNj
# b20xGTAXBgoJkiaJk/IsZAEZFgltaWNyb3NvZnQxLTArBgNVBAMTJE1pY3Jvc29m
# dCBSb290IENlcnRpZmljYXRlIEF1dGhvcml0eTAeFw0wNzA0MDMxMjUzMDlaFw0y
# MTA0MDMxMzAzMDlaMHcxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xITAfBgNVBAMTGE1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQTCCASIwDQYJKoZI
# hvcNAQEBBQADggEPADCCAQoCggEBAJ+hbLHf20iSKnxrLhnhveLjxZlRI1Ctzt0Y
# TiQP7tGn0UytdDAgEesH1VSVFUmUG0KSrphcMCbaAGvoe73siQcP9w4EmPCJzB/L
# MySHnfL0Zxws/HvniB3q506jocEjU8qN+kXPCdBer9CwQgSi+aZsk2fXKNxGU7CG
# 0OUoRi4nrIZPVVIM5AMs+2qQkDBuh/NZMJ36ftaXs+ghl3740hPzCLdTbVK0RZCf
# SABKR2YRJylmqJfk0waBSqL5hKcRRxQJgp+E7VV4/gGaHVAIhQAQMEbtt94jRrvE
# LVSfrx54QTF3zJvfO4OToWECtR0Nsfz3m7IBziJLVP/5BcPCIAsCAwEAAaOCAasw
# ggGnMA8GA1UdEwEB/wQFMAMBAf8wHQYDVR0OBBYEFCM0+NlSRnAK7UD7dvuzK7DD
# NbMPMAsGA1UdDwQEAwIBhjAQBgkrBgEEAYI3FQEEAwIBADCBmAYDVR0jBIGQMIGN
# gBQOrIJgQFYnl+UlE/wq4QpTlVnkpKFjpGEwXzETMBEGCgmSJomT8ixkARkWA2Nv
# bTEZMBcGCgmSJomT8ixkARkWCW1pY3Jvc29mdDEtMCsGA1UEAxMkTWljcm9zb2Z0
# IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5ghB5rRahSqClrUxzWPQHEy5lMFAG
# A1UdHwRJMEcwRaBDoEGGP2h0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3Js
# L3Byb2R1Y3RzL21pY3Jvc29mdHJvb3RjZXJ0LmNybDBUBggrBgEFBQcBAQRIMEYw
# RAYIKwYBBQUHMAKGOGh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMv
# TWljcm9zb2Z0Um9vdENlcnQuY3J0MBMGA1UdJQQMMAoGCCsGAQUFBwMIMA0GCSqG
# SIb3DQEBBQUAA4ICAQAQl4rDXANENt3ptK132855UU0BsS50cVttDBOrzr57j7gu
# 1BKijG1iuFcCy04gE1CZ3XpA4le7r1iaHOEdAYasu3jyi9DsOwHu4r6PCgXIjUji
# 8FMV3U+rkuTnjWrVgMHmlPIGL4UD6ZEqJCJw+/b85HiZLg33B+JwvBhOnY5rCnKV
# uKE5nGctxVEO6mJcPxaYiyA/4gcaMvnMMUp2MT0rcgvI6nA9/4UKE9/CCmGO8Ne4
# F+tOi3/FNSteo7/rvH0LQnvUU3Ih7jDKu3hlXFsBFwoUDtLaFJj1PLlmWLMtL+f5
# hYbMUVbonXCUbKw5TNT2eb+qGHpiKe+imyk0BncaYsk9Hm0fgvALxyy7z0Oz5fns
# fbXjpKh0NbhOxXEjEiZ2CzxSjHFaRkMUvLOzsE1nyJ9C/4B5IYCeFTBm6EISXhrI
# niIh0EPpK+m79EjMLNTYMoBMJipIJF9a6lbvpt6Znco6b72BJ3QGEe52Ib+bgsEn
# VLaxaj2JoXZhtG6hE6a/qkfwEm/9ijJssv7fUciMI8lmvZ0dhxJkAj0tr1mPuOQh
# 5bWwymO0eFQF1EEuUKyUsKV4q7OglnUa2ZKHE3UiLzKoCG6gW4wlv6DvhMoh1use
# T8ma7kng9wFlb4kLfchpyOZu6qeXzjEp/w7FW1zYTRuh2Povnj8uVRZryROj/TGC
# BIkwggSFAgEBMIGQMHkxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xIzAhBgNVBAMTGk1pY3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBAhMzAAABsd3t
# ulTpZbhfAAEAAAGxMAkGBSsOAwIaBQCggaIwGQYJKoZIhvcNAQkDMQwGCisGAQQB
# gjcCAQQwHAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwIwYJKoZIhvcNAQkE
# MRYEFMphrUfwT8dYCfw5IeNdFSbAqf63MEIGCisGAQQBgjcCAQwxNDAyoBSAEgBN
# AGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20wDQYJ
# KoZIhvcNAQEBBQAEggEANw3piiMYfzJxU3VI5CXC5omo66oZp1elgkqU0FZ05yNx
# 2BrYZQJ5KJ8Km4S4vq/aQ7NzGa55Y7MvcPhnA49jQUvIKqAXJnU28YoAjMXpYDC0
# aleOR4pa1dUMv+e4Wdt7OaQMDeR29WE4rRIqMxFGbhFXuWR7G8WccVEPcU6utT/d
# mdN7XBeG6sGhtAGJ/Fs6A3Qp6yNPHXeKCBCYtNuBwIPZ6k8/FlHHCeIhXOPOO6N0
# Q3RB/N1xzZjPgnvVYfE3M7NtxbdAjbTNYJ6XiJcEHPFYBUTwtIZbNs60lZPoPTKD
# aqbW7BZ+is8QE6Pf4revfZlfKRguZJhsYLXLPbziPaGCAigwggIkBgkqhkiG9w0B
# CQYxggIVMIICEQIBATCBjjB3MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGlu
# Z3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBv
# cmF0aW9uMSEwHwYDVQQDExhNaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0ECEzMAAAER
# g1HLZMbeFhAAAAAAAREwCQYFKw4DAhoFAKBdMBgGCSqGSIb3DQEJAzELBgkqhkiG
# 9w0BBwEwHAYJKoZIhvcNAQkFMQ8XDTE4MDkyMjE5NDQxNlowIwYJKoZIhvcNAQkE
# MRYEFKgUbzvO8F5b2Bxs74thslt7aeB5MA0GCSqGSIb3DQEBBQUABIIBAGqF3I8Z
# z79X3bTrLK/0c/IetOxoAVHuI+n6dsn0irh3UcsnwbkfgyyiyOVHccY/P2Bq5DlP
# e5fXoA29GYyb+4Cas0v8ik/zs6c7sRzlA8KKm0wCDEAul6+Jf/uW4K9Qo7pn0kBV
# DXoFqUY7cW35S3lR6mXnNGuscwyTqJlWJljLr+IuJfweON8zG/G5ddbNaLUP7Ztw
# lcIFwf4pe8qvAgGu/yKWIv89lIYctVdjaOosOHAO0mDH/aGEEJQHLhRQFSohH4Vb
# 72ijvu8FiF1ReR6l4OOGphKyUpu0uYiidOEHWg/PNMGNnnA3RuxW0bhaCsQ6Okt0
# ej7wLpTxlzC+tS8=
# SIG # End signature block
