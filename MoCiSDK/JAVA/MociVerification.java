package org.mociCloud;

import java.net.NetworkInterface;
import java.security.MessageDigest;
import java.util.Enumeration;
import java.util.Scanner;

public class MociVerification {
    public static String clientInformation;

    public static String getToekenNotice(String AppToken, String AppVersion) {
        MoCiRequest request = new MoCiRequest("客户", "客户取版本公告", new String[]{
                AppToken,
                AppVersion,
        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        return resultMessage;
    }
    public static String getVersionDate(String AppToken, String AppVersion) {
        MoCiRequest request = new MoCiRequest("客户", "客户取版本数据", new String[]{
                AppToken,
                AppVersion,
        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        return resultMessage;
    }
    public static String getExpirationDate(String AppToken, String AppVersion,String AppCodeKey) {
        MoCiRequest request = new MoCiRequest("客户", "客户取到期时间", new String[]{
                AppToken,
                AppVersion,
                AppCodeKey
        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        return resultMessage;
    }
    public static String isBlacklist(String AppToken, String AppCodeKey ) { //2 黑名单 1 非黑名单
        MoCiRequest request = new MoCiRequest("客户", "客户是否黑名", new String[]{
                AppToken,
                AppCodeKey,

        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        return resultMessage;

    }
    public static String addBlacklist(String AppToken,String Reason,String Key ) {
        MoCiRequest request = new MoCiRequest("客户", "客户添加黑名", new String[]{
                AppToken,
                Key,
                Reason

        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        return resultMessage;
    }
    public static String getDataNoVerify(String AppToken, String AppVersion,String ApplyKey) {
        MoCiRequest request = new MoCiRequest("客户", "客户取自定义数据", new String[]{
                AppToken,
                AppVersion,
                ApplyKey,
                "未登录状态"

        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        return resultMessage;

    }
    public static String getLatestVersion(String AppToken) {
        MoCiRequest request = new MoCiRequest("客户", "客户取最新版本", new String[]{
                AppToken,

        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        return resultMessage;
    }
    public static String getProjectNotice(String AppToken) {
        MoCiRequest request = new MoCiRequest("客户", "客户取项目公告", new String[]{
                AppToken,

        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        return resultMessage;
    }
    public static String getUserCounts(String AppToken) {
        MoCiRequest request = new MoCiRequest("客户", "客户取全网在线用户", new String[]{
                AppToken,

        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        System.out.println(resultMessage);
        return resultMessage;
    }










    public static boolean Verify(String AppToken, String AppVersion, String AppCodeKey, String HWID) {
        String userInput =AppCodeKey;
        MoCiRequest request = new MoCiRequest("客户", "客户单码登录", new String[]{
                AppToken,
                AppVersion,
                userInput,
                HWID
        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        if (resultMessage.contains("登录成功")){
            return true;
        }else{
            return false;
        }
    }
    public static boolean exitVerify(String AppToken, String AppVersion, String AppCodeKey) {
        String userInput = AppCodeKey;
        MoCiRequest request = new MoCiRequest("客户", "客户退登", new String[]{
                AppToken,
                AppVersion,
                userInput
        });
        String resultMessage = request.getResult();
        clientInformation =  request.getResult();
        if (resultMessage.contains("退登成功")){
            return true;
        }else{
            return false;
        }
    }







    public static String generateHWID() {
        StringBuilder sb = new StringBuilder();
        try {
            sb.append(getMacAddress());
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
        sb.append(getProcessorId());
        sb.append(getDiskSerial());
        sb.append(getBaseboardSerial());
        sb.append(System.getProperty("os.name"));
        sb.append(System.getProperty("os.arch"));
        sb.append(System.getProperty("os.version"));
        byte[] hash = new byte[0];
        try {
            MessageDigest md = MessageDigest.getInstance("SHA-256");
            hash = md.digest(sb.toString().getBytes("UTF-8"));
        } catch (Exception e) {

        }


        StringBuilder hexString = new StringBuilder();
        for (byte b : hash) {
            String hex = Integer.toHexString(0xff & b);
            if (hex.length() == 1) hexString.append('0');
            hexString.append(hex);
        }

        return hexString.toString();
    }

    private static String getMacAddress() throws Exception {
        Enumeration<NetworkInterface> networkInterfaces = NetworkInterface.getNetworkInterfaces();
        while (networkInterfaces.hasMoreElements()) {
            NetworkInterface ni = networkInterfaces.nextElement();
            byte[] mac = ni.getHardwareAddress();
            if (mac != null) {
                StringBuilder sb = new StringBuilder();
                for (byte b : mac) {
                    sb.append(String.format("%02X", b));
                }
                return sb.toString();
            }
        }
        return "";
    }

    private static String getProcessorId() {
        try {
            Process process = Runtime.getRuntime().exec(
                    new String[] { "wmic", "cpu", "get", "ProcessorId" });
            process.getOutputStream().close();
            Scanner sc = new Scanner(process.getInputStream());
            sc.next();
            String serial = sc.next();
            sc.close();
            return serial;
        } catch (Exception e) {
            return "";
        }
    }

    private static String getDiskSerial() {
        try {
            Process process = Runtime.getRuntime().exec(
                    new String[] { "wmic", "diskdrive", "get", "serialnumber" });
            process.getOutputStream().close();
            Scanner sc = new Scanner(process.getInputStream());
            sc.next();
            String serial = sc.next();
            sc.close();
            return serial;
        } catch (Exception e) {
            return "";
        }
    }

    private static String getBaseboardSerial() {
        try {
            Process process = Runtime.getRuntime().exec(
                    new String[] { "wmic", "baseboard", "get", "serialnumber" });
            process.getOutputStream().close();
            Scanner sc = new Scanner(process.getInputStream());
            sc.next();
            String serial = sc.next();
            sc.close();
            return serial;
        } catch (Exception e) {
            return "";
        }
    }




}
