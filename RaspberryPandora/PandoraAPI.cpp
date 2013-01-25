/* 
 * File:   PandoraAPI.cpp
 * Author: brian
 * 
 * Created on January 24, 2013, 9:43 PM
 */

#include "PandoraAPI.h"
#include <iostream>
#include <curlpp/cURLpp.hpp>
#include <curlpp/Easy.hpp>
#include <curlpp/Options.hpp>

using namespace std;

PandoraAPI::PandoraAPI() {
	partnerLogin();
}

PandoraAPI::PandoraAPI(string userName, string password){
    partnerLogin();
    userLogin(userName, password);
}

PandoraAPI::~PandoraAPI() {
}

bool PandoraAPI::partnerLogin(){
	string data = "{"
		"\"username\": \"android\","
		"\"password\": \"AC7IBG09A3DTSYM4R41UJWL07VLN8JI7\","
		"\"deviceModel\": \"android-generic\","
		"\"version\": \"5\""
	"}";
	int size = data.length();
	char buf[size];
	
	curlpp::Easy req;
	list<string> headers;
	headers.push_back("Content-Type: application/json");
	sprintf(buf, "Content-Length: %d", size); 
	headers.push_back(buf);
	
	using namespace curlpp::options;
	req.setOpt(HttpPost);
	req.setOpt(new HttpHeader(headers));
	req.setOpt(new Upload(true));
	req.setOpt(new ReadFunction(curlpp::types::ReadFunctionFunctor(readData)));
	req.setOpt(Url("http://tuner.pandora.com/services/json/?method=auth.partnerLogin"));
	
	req.perform();
    return true;
}

bool PandoraAPI::userLogin(string userName, string password){
    return true;
}
    
void PandoraAPI::logOut(){
    cout << "Logging Out" << endl;
}

char *data = NULL;
size_t readData(char *buffer, size_t size, size_t nitems)
{
	strncpy(buffer, data, size * nitems);
	cout << data << endl;
}