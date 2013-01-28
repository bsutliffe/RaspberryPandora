/* 
 * File:   PandoraAPI.h
 * Author: brian
 *
 * Created on January 24, 2013, 9:43 PM
 */

#ifndef PANDORAAPI_H
#define	PANDORAAPI_H

#include <stddef.h>
#include <string>
#include "Blowfish/Blowfish.h"

using namespace std;

class APIWrapper {
public:
	APIWrapper();
	APIWrapper(string, string);
	bool userLogin(string, string);
	void logOut();
	virtual ~APIWrapper();
private:
	string partnerId;
	string partnerAuthToken;
	string userAuthToken;
	int syncTime;
	int startTime;
	CBlowFish* encryptor;
	CBlowFish* decryptor;
	void standardSetup();
	bool partnerLogin();
	string makeRequest(string, string, bool);
	void decryptSyncTime(const char*);
};

#endif	/* PANDORAAPI_H */

