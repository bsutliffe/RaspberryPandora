/* 
 * File:   APIWrapper.cpp
 * Author: brian
 * 
 * Created on January 24, 2013, 9:43 PM
 */

#define CURL_STATICLIB
#include "APIWrapper.h"
#include "Blowfish/Blowfish.h"
#include <iostream>
#include <cstring>
#include <curl/curl.h>
#include <curl/types.h>
#include <curl/easy.h>
#include <json/json.h>
#include <boost/lexical_cast.hpp>

using namespace std;

namespace {

	size_t write_to_string(void *ptr, size_t size, size_t count, void *stream) {
		((string*) stream)->append((char*) ptr, 0, size * count);
		return size*count;
	}

	//Function to convert unsigned char to string of length 2

	void Char2Hex(const unsigned char ch, char* szHex) {
		unsigned char byte[2];
		byte[0] = ch / 16;
		byte[1] = ch % 16;
		for (int i = 0; i < 2; i++) {
			if (byte[i] >= 0 && byte[i] <= 9)
				szHex[i] = '0' + byte[i];
			else
				szHex[i] = 'A' + byte[i] - 10;
		}
		szHex[2] = 0;
	}

	//Function to convert string of length 2 to unsigned char

	void Hex2Char(const char* szHex, unsigned char& rch) {
		rch = 0;
		for (int i = 0; i < 2; i++) {
			if (*(szHex + i) >= '0' && *(szHex + i) <= '9')
				rch = (rch << 4) + (*(szHex + i) - '0');
			else if (toupper(*(szHex + i)) >= 'A' && toupper(*(szHex + i)) <= 'F')
				rch = (rch << 4) + (toupper(*(szHex + i)) - 'A' + 10);
			else
				break;
		}
	}

	//Function to convert string of unsigned chars to string of chars

	void CharStr2HexStr(const unsigned char* pucCharStr, char* pszHexStr, int iSize) {
		int i;
		char szHex[3];
		pszHexStr[0] = 0;
		for (i = 0; i < iSize; i++) {
			Char2Hex(pucCharStr[i], szHex);
			strcat(pszHexStr, szHex);
		}
	}

	//Function to convert string of chars to string of unsigned chars

	void HexStr2CharStr(const char* pszHexStr, unsigned char* pucCharStr, int iSize) {
		int i;
		unsigned char ch;
		for (i = 0; i < iSize; i++) {
			Hex2Char(pszHexStr + 2 * i, ch);
			pucCharStr[i] = ch;
		}
	}

	int roundUp(int numToRound, int multiple) {
		if (multiple == 0) {
			return numToRound;
		}

		int remainder = numToRound % multiple;
		if (remainder == 0)
			return numToRound;
		return numToRound + multiple - remainder;
	}
}

APIWrapper::APIWrapper() {
	standardSetup();
	partnerLogin();
}

APIWrapper::APIWrapper(string userName, string password) {
	standardSetup();
	partnerLogin();
	userLogin(userName, password);
}

APIWrapper::~APIWrapper() {
	//delete encryptor;
	//delete decryptor;
}

void APIWrapper::standardSetup() {
	time_t seconds;
	seconds = time(NULL);
	startTime = static_cast<int> (seconds);
	encryptor = new CBlowFish((unsigned char*) "6#26FRL$ZWD", (size_t) 11);
	decryptor = new CBlowFish((unsigned char*) "R=U!LH$O2B#", (size_t) 11);
}

bool APIWrapper::partnerLogin() {
	string data = "{"
			"\"username\": \"android\","
			"\"password\": \"AC7IBG09A3DTSYM4R41UJWL07VLN8JI7\","
			"\"deviceModel\": \"android-generic\","
			"\"version\": \"5\""
			"}";

	string response = makeRequest("http://tuner.pandora.com/services/json/?method=auth.partnerLogin", data, false);
	Json::Value root;
	Json::Reader reader;
	bool parsingSuccessful = reader.parse(response, root);
	if (!parsingSuccessful)
		return false;
	if (root["stat"] != "ok")
		return false;
	cout << response << endl;
	partnerId = root["result"]["partnerId"].asString();
	partnerAuthToken = root["result"]["partnerAuthToken"].asString();
	decryptSyncTime(root["result"]["syncTime"].asCString());
	return true;
}

bool APIWrapper::userLogin(string userName, string password) {
	time_t now;
	now = time(NULL);
	int curSyncTime = syncTime + (static_cast<int> (now) - startTime);
	string data = "{"
		"\"loginType\":\"user\","
		"\"username\":\"" + userName + "\","
		"\"password\":\"" + password + "\","
		"\"partnerAuthToken\":\"" + partnerAuthToken + "\","
		"\"syncTime\":" + boost::lexical_cast<string > (curSyncTime) + ""
	"}";
	string response = makeRequest("http://tuner.pandora.com/services/json/?method=auth.userLogin&auth_token=" + boost::lexical_cast<string > (curl_escape(partnerAuthToken.c_str(), partnerAuthToken.length())) + "&partner_id=" + partnerId, data, true);
	cout << response << endl;
	Json::Value root;
	Json::Reader reader;
	bool parsingSuccessful = reader.parse(response, root);
	if (!parsingSuccessful)
		return false;
	if (root["stat"] != "ok")
		return false;
	return true;
}

void APIWrapper::logOut() {
	cout << "Logging Out" << endl;
}

string APIWrapper::makeRequest(string url, string body, bool encrypt) {
	cout << url << endl;
	if (encrypt) {
		const char* bodyChar = body.c_str();
		cout << bodyChar << endl;
		int len = roundUp(strlen(bodyChar), 8);
		char encrypted[len];
		char hex[len*2];
		char decrypted[len];
		memset(encrypted, 0, len);
		encryptor->Encrypt((const unsigned char*)body.c_str(), (unsigned char*) encrypted, len);
		cout << encrypted << endl;
		CharStr2HexStr((unsigned char*) encrypted, hex, len);
		body = boost::lexical_cast<string > ((const char*) hex);
		cout << endl << body << endl;
		return "";
	}
	//cout << endl;
	CURL *curl = curl_easy_init();
	string response;
	struct curl_slist *headers = NULL;
	headers = curl_slist_append(headers, "Accept: application/json");
	headers = curl_slist_append(headers, "Content-Type: application/json");
	headers = curl_slist_append(headers, "charsets: utf-8");
	curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);
	curl_easy_setopt(curl, CURLOPT_HTTPPOST, 1);
	curl_easy_setopt(curl, CURLOPT_POSTFIELDS, body.c_str());
	curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
	curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, write_to_string);
	curl_easy_setopt(curl, CURLOPT_WRITEDATA, &response);

	CURLcode res = curl_easy_perform(curl);
	curl_easy_cleanup(curl);

	return response;
}

void APIWrapper::decryptSyncTime(const char* encrypted) {
	try {
		//first, de-hex the encrypted string
		char dehexed[16];
		HexStr2CharStr(encrypted, (unsigned char*) dehexed, 16);
		int len = roundUp(strlen(dehexed), 8);
		//then decrypt
		char decrypted[len];
		decryptor->Decrypt((const unsigned char*) dehexed, (unsigned char*) decrypted, len);
		//then trim off the first 4 bytes of garbage and take the next 10
		char trimmed[11];
		for(int x = 4; x < 15; x++){
			trimmed[x - 4] = decrypted[x];
		}
		//finally, convert to an integer
		sscanf((const char*) trimmed, "%d", &syncTime);
	} catch (int e) {
		switch (e) {
			case 1:
				cout << "Incorrect key length" << endl;
				break;
			case 2:
				cout << "Incorrect buffer length" << endl;
				break;
		}
	}
}