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
		((string*)stream)->append((char*)ptr, 0, size*count);
		return size*count;
	}
	
	char *strToHex(unsigned char *str, int len){
		char *buffer = new char[len*2+1];
		char *pbuffer = buffer;
		for(int i = 0; i < len ; ++i ){
		  sprintf(pbuffer, "%02X", str[i]);
		  pbuffer += 2;
		}
		return buffer;
	}
}

APIWrapper::APIWrapper() {
	standardSetup();
	partnerLogin();
}

APIWrapper::APIWrapper(string userName, string password){
	standardSetup();
	partnerLogin();
	userLogin(userName, password);
}

APIWrapper::~APIWrapper() {
	//delete encryptor;
	//delete decryptor;
}

void APIWrapper::standardSetup(){
	time_t seconds;
	seconds = time(NULL);
	startTime = static_cast<int>(seconds);
	encryptor = new CBlowFish((unsigned char*)"6#26FRL$ZWD", (size_t)11);
	decryptor = new CBlowFish((unsigned char*)"R=U!LH$O2B#", (size_t)11);
}

bool APIWrapper::partnerLogin(){
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
	if(!parsingSuccessful)
		return false;
	if(root["stat"] != "ok")
		return false;
	partnerId = root["result"]["partnerId"].asString();
	partnerAuthToken = root["result"]["partnerAuthToken"].asString();
	decryptSyncTime(root["result"]["syncTime"].asCString());
	return true;
}

bool APIWrapper::userLogin(string userName, string password){
	time_t now;
	now = time(NULL);
	int curSyncTime = syncTime + (static_cast<int>(now) - startTime);
	string data = "{"
		"\"loginType\": \"user\","
		"\"username\":\"" + userName + "\","
		"\"password\":\"" + password + "\","
		"\"partnerAuthToken\":\"" + partnerAuthToken + "\","
		"\"syncTime\": " + boost::lexical_cast<string>(curSyncTime) + ""
	"}";
	cout << partnerAuthToken << endl;
	string response = makeRequest("http://tuner.pandora.com/services/json/?method=auth.userLogin&auth_token=" +  boost::lexical_cast<string>(curl_escape(partnerAuthToken.c_str(), partnerAuthToken.length())) + "&partner_id=" + partnerId, data, true);
	cout << response << endl;
	Json::Value root;
	Json::Reader reader;
	bool parsingSuccessful = reader.parse(response, root);
	if(!parsingSuccessful)
		return false;
	if(root["stat"] != "ok")
		return false;
	return true;
}
    
void APIWrapper::logOut(){
	cout << "Logging Out" << endl;
}

string APIWrapper::makeRequest(string url, string body, bool encrypt){
	cout << body << endl;
	if(encrypt){
		char encrypted[1024];
		memset(encrypted, 0, 1024);
		encryptor->Encrypt((unsigned char*)body.c_str(), (unsigned char*)encrypted, 1024, CBlowFish::ECB);
		body = strToHex((unsigned char*)encrypted, 1024);
		cout << body << endl;
	}
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

void APIWrapper::decryptSyncTime(const char* encrypted){
	try{
		char decrypted[16];
		memset(decrypted, 0, 16);
		decryptor->Decrypt((unsigned char*)decrypted, (unsigned char*)encrypted, 16, CBlowFish::ECB);
		sscanf((const char*)decrypted, "%d", &syncTime);
	} catch(int e) {
		switch(e){
			case 1:
				cout << "Incorrect key length" << endl;
				break;
			case 2:
				cout << "Incorrect buffer length" << endl;
				break;
		}
	}
}