#include "pch.h"
#include "Moci.h"
#include <sstream>
#include <iomanip>
#include <iphlpapi.h>
#include <comdef.h>
#include <winhttp.h>
#include <wincrypt.h>
#include <algorithm>
#include <wbemidl.h>
#include <windows.h>
#include <stdexcept>

#pragma comment(lib, "iphlpapi.lib")
#pragma comment(lib, "winhttp.lib")
#pragma comment(lib, "wbemuuid.lib")
#pragma comment(lib, "crypt32.lib")

namespace org {
    namespace moci {
        std::string Moci::clientInformation;

        std::string Moci::getToekenNotice(const std::string& AppToken, const std::string& AppVersion) {
            std::vector<std::string> params = { AppToken, AppVersion };
            MociRequest request("客户", "客户取版本公告", params);
            std::string resultMessage = convert(request.getResult());
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::getVersionDate(const std::string& AppToken, const std::string& AppVersion) {
            std::vector<std::string> params = { AppToken, AppVersion };
            MociRequest request("客户", "客户取版本数据", params);
            std::string resultMessage = convert(request.getResult());
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::getExpirationDate(const std::string& AppToken, const std::string& AppVersion, const std::string& AppCodeKey) {
            std::vector<std::string> params = { AppToken, AppVersion, AppCodeKey };
            MociRequest request("客户", "客户取到期时间", params);
            std::string resultMessage = request.getResult();
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::isBlacklist(const std::string& AppToken, const std::string& AppCodeKey) {
            std::vector<std::string> params = { AppToken, AppCodeKey };
            MociRequest request("客户", "客户是否黑名", params);
            std::string resultMessage = request.getResult();
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::addBlacklist(const std::string& AppToken, const std::string& Reason, const std::string& Key) {
            std::vector<std::string> params = { AppToken, Key, Reason };
            MociRequest request("客户", "客户添加黑名", params);
            std::string resultMessage = request.getResult();
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::getDataNoVerify(const std::string& AppToken, const std::string& AppVersion, const std::string& ApplyKey) {
            std::vector<std::string> params = { AppToken, AppVersion, ApplyKey, "未登录状态" };
            MociRequest request("客户", "客户取自定义数据", params);
            std::string resultMessage = convert(request.getResult());
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::getLatestVersion(const std::string& AppToken) {
            std::vector<std::string> params = { AppToken };
            MociRequest request("客户", "客户取最新版本", params);
            std::string resultMessage = request.getResult();
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::getProjectNotice(const std::string& AppToken) {
            std::vector<std::string> params = { AppToken };
            MociRequest request("客户", "客户取项目公告", params);
            std::string resultMessage = convert(request.getResult());
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::getUserCounts(const std::string& AppToken) {
            std::vector<std::string> params = { AppToken };
            MociRequest request("客户", "客户取全网在线用户", params);
            std::string resultMessage = convert(request.getResult());
            clientInformation = request.getResult();
            return resultMessage;
        }

        std::string Moci::convert(const std::string& input) {
            std::string output = input;
            for (size_t i = 0; i < output.length(); ++i) {
                if (output[i] == '+') {
                    output[i] = ' ';
                }
            }
            return output;
        }

        bool Moci::Verify(const std::string& AppToken, const std::string& AppVersion, const std::string& AppCodeKey, const std::string& HWID) {
            std::vector<std::string> params = { AppToken, AppVersion, AppCodeKey, HWID };
            MociRequest request("客户", "客户单码登录", params);
            std::string resultMessage = request.getResult();
            clientInformation = request.getResult();
            return resultMessage.find("登录成功") != std::string::npos;
        }

        bool Moci::exitVerify(const std::string& AppToken, const std::string& AppVersion, const std::string& AppCodeKey) {
            std::vector<std::string> params = { AppToken, AppVersion, AppCodeKey };
            MociRequest request("客户", "客户退登", params);
            std::string resultMessage = request.getResult();
            clientInformation = request.getResult();
            return resultMessage.find("退登成功") != std::string::npos;
        }

        std::string Moci::generateHWID() {
            std::stringstream sb;
            try {
                sb << getMacAddress();
            }
            catch (const std::exception& e) {
                throw std::runtime_error(e.what());
            }
            sb << getProcessorId();
            sb << getDiskSerial();
            sb << getBaseboardSerial();

            OSVERSIONINFOEX osvi = { 0 };
            osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFOEX);

            DWORDLONG dwlConditionMask = 0;
            VER_SET_CONDITION(dwlConditionMask, VER_MAJORVERSION, VER_GREATER_EQUAL);
            VER_SET_CONDITION(dwlConditionMask, VER_MINORVERSION, VER_GREATER_EQUAL);

            osvi.dwMajorVersion = 5;
            osvi.dwMinorVersion = 0;

            DWORD dwMajor = 0, dwMinor = 0, dwBuild = 0;

            if (VerifyVersionInfo(&osvi, VER_MAJORVERSION | VER_MINORVERSION, dwlConditionMask)) {
                HMODULE hKernel = GetModuleHandle(L"kernel32.dll");
                if (hKernel) {
                    typedef LONG(WINAPI* RtlGetVersionFunc)(LPOSVERSIONINFOEX);
                    RtlGetVersionFunc RtlGetVersion = (RtlGetVersionFunc)GetProcAddress(hKernel, "RtlGetVersion");
                    if (RtlGetVersion) {
                        RtlGetVersion(&osvi);
                        dwMajor = osvi.dwMajorVersion;
                        dwMinor = osvi.dwMinorVersion;
                        dwBuild = osvi.dwBuildNumber;
                    }
                }
            }

            SYSTEM_INFO si;
            GetSystemInfo(&si);

            std::string osArch;
            switch (si.wProcessorArchitecture) {
            case PROCESSOR_ARCHITECTURE_AMD64: osArch = "x64"; break;
            case PROCESSOR_ARCHITECTURE_INTEL: osArch = "x86"; break;
            case PROCESSOR_ARCHITECTURE_ARM: osArch = "ARM"; break;
            default: osArch = "Unknown";
            }

            sb << "Windows " << dwMajor << "." << dwMinor;
            sb << osArch;
            sb << dwBuild;

            return sha256(sb.str());
        }

        std::string Moci::getMacAddress() {
            IP_ADAPTER_INFO* pAdapterInfo;
            ULONG ulOutBufLen = sizeof(IP_ADAPTER_INFO);
            pAdapterInfo = (IP_ADAPTER_INFO*)LocalAlloc(LPTR, ulOutBufLen);

            if (GetAdaptersInfo(pAdapterInfo, &ulOutBufLen) == ERROR_BUFFER_OVERFLOW) {
                LocalFree(pAdapterInfo);
                pAdapterInfo = (IP_ADAPTER_INFO*)LocalAlloc(LPTR, ulOutBufLen);
            }

            if (GetAdaptersInfo(pAdapterInfo, &ulOutBufLen) == NO_ERROR) {
                for (IP_ADAPTER_INFO* pAdapter = pAdapterInfo; pAdapter; pAdapter = pAdapter->Next) {
                    if (pAdapter->Type == MIB_IF_TYPE_ETHERNET) {
                        std::stringstream ss;
                        for (UINT i = 0; i < pAdapter->AddressLength; ++i) {
                            ss << std::setw(2) << std::setfill('0') << std::hex << (int)pAdapter->Address[i];
                        }
                        LocalFree(pAdapterInfo);
                        return ss.str();
                    }
                }
            }

            LocalFree(pAdapterInfo);
            return "";
        }

        std::string Moci::getProcessorId() {
            return getWMIProperty(L"Win32_Processor", L"ProcessorId");
        }

        std::string Moci::getDiskSerial() {
            return getWMIProperty(L"Win32_DiskDrive", L"SerialNumber");
        }

        std::string Moci::getBaseboardSerial() {
            return getWMIProperty(L"Win32_BaseBoard", L"SerialNumber");
        }

        std::string Moci::getWMIProperty(const std::wstring& wmiClass, const std::wstring& wmiProperty) {
            HRESULT hres;

            hres = CoInitializeEx(0, COINIT_APARTMENTTHREADED);
            if (FAILED(hres)) {
                return "";
            }

            hres = CoInitializeSecurity(
                NULL,
                -1,
                NULL,
                NULL,
                RPC_C_AUTHN_LEVEL_DEFAULT,
                RPC_C_IMP_LEVEL_IMPERSONATE,
                NULL,
                EOAC_NONE,
                NULL
            );

            if (FAILED(hres)) {
                CoUninitialize();
                return "";
            }

            IWbemLocator* pLoc = NULL;
            hres = CoCreateInstance(
                CLSID_WbemLocator,
                0,
                CLSCTX_INPROC_SERVER,
                IID_IWbemLocator,
                (LPVOID*)&pLoc
            );

            if (FAILED(hres)) {
                CoUninitialize();
                return "";
            }

            IWbemServices* pSvc = NULL;
            hres = pLoc->ConnectServer(
                _bstr_t(L"ROOT\\CIMV2"),
                NULL,
                NULL,
                0,
                NULL,
                0,
                0,
                &pSvc
            );

            if (FAILED(hres)) {
                pLoc->Release();
                CoUninitialize();
                return "";
            }

            hres = CoSetProxyBlanket(
                pSvc,
                RPC_C_AUTHN_WINNT,
                RPC_C_AUTHZ_NONE,
                NULL,
                RPC_C_AUTHN_LEVEL_CALL,
                RPC_C_IMP_LEVEL_IMPERSONATE,
                NULL,
                EOAC_NONE
            );

            if (FAILED(hres)) {
                pSvc->Release();
                pLoc->Release();
                CoUninitialize();
                return "";
            }

            IEnumWbemClassObject* pEnumerator = NULL;
            std::wstring query = L"SELECT " + wmiProperty + L" FROM " + wmiClass;
            hres = pSvc->ExecQuery(
                bstr_t("WQL"),
                bstr_t(query.c_str()),
                WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
                NULL,
                &pEnumerator
            );

            if (FAILED(hres)) {
                pSvc->Release();
                pLoc->Release();
                CoUninitialize();
                return "";
            }

            IWbemClassObject* pclsObj = NULL;
            ULONG uReturn = 0;
            std::string result;

            while (pEnumerator) {
                HRESULT hr = pEnumerator->Next(WBEM_INFINITE, 1, &pclsObj, &uReturn);
                if (0 == uReturn) {
                    break;
                }

                VARIANT vtProp;
                hr = pclsObj->Get(wmiProperty.c_str(), 0, &vtProp, 0, 0);
                if (!FAILED(hr) && vtProp.vt == VT_BSTR) {
                    _bstr_t bstr(vtProp.bstrVal);
                    result = std::string((const char*)bstr);
                }
                VariantClear(&vtProp);
                pclsObj->Release();
                break;
            }

            pEnumerator->Release();
            pSvc->Release();
            pLoc->Release();
            CoUninitialize();

            result.erase(remove_if(result.begin(), result.end(), isspace), result.end());
            return result;
        }

        std::string Moci::sha256(const std::string& input) {
            HCRYPTPROV hProv = 0;
            HCRYPTHASH hHash = 0;
            BYTE digest[32];
            DWORD digestLen = 32;

            if (!CryptAcquireContext(&hProv, NULL, NULL, PROV_RSA_AES, CRYPT_VERIFYCONTEXT)) {
                return "";
            }

            if (!CryptCreateHash(hProv, CALG_SHA_256, 0, 0, &hHash)) {
                CryptReleaseContext(hProv, 0);
                return "";
            }

            if (!CryptHashData(hHash, (BYTE*)input.c_str(), input.size(), 0)) {
                CryptDestroyHash(hHash);
                CryptReleaseContext(hProv, 0);
                return "";
            }

            if (!CryptGetHashParam(hHash, HP_HASHVAL, digest, &digestLen, 0)) {
                CryptDestroyHash(hHash);
                CryptReleaseContext(hProv, 0);
                return "";
            }

            std::stringstream ss;
            for (DWORD i = 0; i < digestLen; ++i) {
                ss << std::setw(2) << std::setfill('0') << std::hex << (int)digest[i];
            }

            CryptDestroyHash(hHash);
            CryptReleaseContext(hProv, 0);

            return ss.str();
        }

        MociRequest::MociRequest(const std::string& header, const std::string& type, const std::vector<std::string>& parameters) {
            std::stringstream sb;
            sb << header << "|||" << type;
            for (const std::string& param : parameters) {
                sb << "|||" << param;
            }
            try {
                std::string requestData = getRequestData(sb.str());
                this->result = webRequest(requestData);
            }
            catch (const std::exception& e) {
                this->result = "{\"error\": \"Request failed: " + std::string(e.what()) + "\"}";
            }
        }

        std::string MociRequest::getResult() const {
            return result;
        }

        std::string MociRequest::webRequest(const std::string& requestUrl) {
            HINTERNET hSession = WinHttpOpen(L"Mozilla/5.0", WINHTTP_ACCESS_TYPE_DEFAULT_PROXY, WINHTTP_NO_PROXY_NAME, WINHTTP_NO_PROXY_BYPASS, 0);
            if (!hSession) {
                throw std::runtime_error("WinHttpOpen failed");
            }

            std::wstring urlWstr(requestUrl.begin(), requestUrl.end());
            LPCWSTR pwszUrl = urlWstr.c_str();

            URL_COMPONENTS urlComp;
            ZeroMemory(&urlComp, sizeof(urlComp));
            urlComp.dwStructSize = sizeof(urlComp);

            wchar_t szHostName[256] = { 0 };
            urlComp.lpszHostName = szHostName;
            urlComp.dwHostNameLength = sizeof(szHostName) / sizeof(wchar_t);

            wchar_t szUrlPath[1024] = { 0 };
            urlComp.lpszUrlPath = szUrlPath;
            urlComp.dwUrlPathLength = sizeof(szUrlPath) / sizeof(wchar_t);

            wchar_t szExtraInfo[1024] = { 0 };
            urlComp.lpszExtraInfo = szExtraInfo;
            urlComp.dwExtraInfoLength = sizeof(szExtraInfo) / sizeof(wchar_t);

            if (!WinHttpCrackUrl(pwszUrl, 0, 0, &urlComp)) {
                WinHttpCloseHandle(hSession);
                throw std::runtime_error("WinHttpCrackUrl failed");
            }

            HINTERNET hConnect = WinHttpConnect(hSession, urlComp.lpszHostName, urlComp.nPort, 0);
            if (!hConnect) {
                WinHttpCloseHandle(hSession);
                throw std::runtime_error("WinHttpConnect failed");
            }

            std::wstring pathWithQuery = std::wstring(urlComp.lpszUrlPath) + std::wstring(urlComp.lpszExtraInfo);
            HINTERNET hRequest = WinHttpOpenRequest(hConnect, L"GET", pathWithQuery.c_str(), NULL, WINHTTP_NO_REFERER, WINHTTP_DEFAULT_ACCEPT_TYPES, (urlComp.nPort == 443) ? WINHTTP_FLAG_SECURE : 0);
            if (!hRequest) {
                WinHttpCloseHandle(hConnect);
                WinHttpCloseHandle(hSession);
                throw std::runtime_error("WinHttpOpenRequest failed");
            }

            DWORD dwFlags = SECURITY_FLAG_IGNORE_UNKNOWN_CA |
                SECURITY_FLAG_IGNORE_CERT_WRONG_USAGE |
                SECURITY_FLAG_IGNORE_CERT_CN_INVALID |
                SECURITY_FLAG_IGNORE_CERT_DATE_INVALID;
            WinHttpSetOption(hRequest, WINHTTP_OPTION_SECURITY_FLAGS, &dwFlags, sizeof(dwFlags));

            if (!WinHttpSendRequest(hRequest, WINHTTP_NO_ADDITIONAL_HEADERS, 0, WINHTTP_NO_REQUEST_DATA, 0, 0, 0)) {
                WinHttpCloseHandle(hRequest);
                WinHttpCloseHandle(hConnect);
                WinHttpCloseHandle(hSession);
                throw std::runtime_error("WinHttpSendRequest failed");
            }

            if (!WinHttpReceiveResponse(hRequest, NULL)) {
                WinHttpCloseHandle(hRequest);
                WinHttpCloseHandle(hConnect);
                WinHttpCloseHandle(hSession);
                throw std::runtime_error("WinHttpReceiveResponse failed");
            }

            DWORD dwSize = 0;
            WinHttpQueryDataAvailable(hRequest, &dwSize);

            std::vector<BYTE> buffer(dwSize + 1);
            ZeroMemory(buffer.data(), buffer.size());

            DWORD dwDownloaded = 0;
            if (!WinHttpReadData(hRequest, buffer.data(), dwSize, &dwDownloaded)) {
                WinHttpCloseHandle(hRequest);
                WinHttpCloseHandle(hConnect);
                WinHttpCloseHandle(hSession);
                throw std::runtime_error("WinHttpReadData failed");
            }

            WinHttpCloseHandle(hRequest);
            WinHttpCloseHandle(hConnect);
            WinHttpCloseHandle(hSession);

            return std::string(reinterpret_cast<char*>(buffer.data()));
        }

        std::string MociRequest::getRequestData(const std::string& data) {
            std::string serverAddress = "https://ver-cnode.niansir.com/v2/api.php";
            std::string encodedData = urlEncode(data);
            return serverAddress + "?parm=" + encodedData;
        }

        std::string MociRequest::urlEncode(const std::string& value) {
            static const std::string unreserved = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.~";

            std::stringstream encoded;
            encoded.fill('0');
            encoded << std::hex;

            for (char c : value) {
                if (unreserved.find(c) != std::string::npos) {
                    encoded << c;
                }
                else {
                    encoded << '%' << std::setw(2) << int((unsigned char)c);
                }
            }

            return encoded.str();
        }
    }
}
