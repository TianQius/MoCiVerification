#pragma once
#include <string>
#include <vector>

namespace org {
    namespace moci {
        // Ç°ÏòÉùÃ÷
        class MociRequest;

        class Moci {
        public:
            static std::string clientInformation;

            static std::string getToekenNotice(const std::string& AppToken, const std::string& AppVersion);
            static std::string getVersionDate(const std::string& AppToken, const std::string& AppVersion);
            static std::string getExpirationDate(const std::string& AppToken, const std::string& AppVersion, const std::string& AppCodeKey);
            static std::string isBlacklist(const std::string& AppToken, const std::string& AppCodeKey);
            static std::string addBlacklist(const std::string& AppToken, const std::string& Reason, const std::string& Key);
            static std::string getDataNoVerify(const std::string& AppToken, const std::string& AppVersion, const std::string& ApplyKey);
            static std::string getLatestVersion(const std::string& AppToken);
            static std::string getProjectNotice(const std::string& AppToken);
            static std::string getUserCounts(const std::string& AppToken);
            static std::string convert(const std::string& input);
            static bool Verify(const std::string& AppToken, const std::string& AppVersion, const std::string& AppCodeKey, const std::string& HWID);
            static bool exitVerify(const std::string& AppToken, const std::string& AppVersion, const std::string& AppCodeKey);
            static std::string generateHWID();
            static std::string getMacAddress();
            static std::string getProcessorId();
            static std::string getDiskSerial();
            static std::string getBaseboardSerial();
            static std::string getWMIProperty(const std::wstring& wmiClass, const std::wstring& wmiProperty);
            static std::string sha256(const std::string& input);
        };

        class MociRequest {
        private:
            std::string result;
            std::string webRequest(const std::string& requestUrl);
            std::string getRequestData(const std::string& data);
            std::string urlEncode(const std::string& value);

        public:
            MociRequest(const std::string& header, const std::string& type, const std::vector<std::string>& parameters);
            std::string getResult() const;
        };
    }
}
