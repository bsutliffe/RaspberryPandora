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

using namespace std;

class PandoraAPI {
public:
	PandoraAPI();
	PandoraAPI(string, string);
	bool userLogin(string, string);
	void logOut();
	virtual ~PandoraAPI();
private:
	bool partnerLogin();
};

#endif	/* PANDORAAPI_H */

