/* 
 * File:   Main.cpp
 * Author: brian
 *
 * Created on January 24, 2013, 9:37 PM
 */

#include <cstdlib>
#include <iostream>
#include <string>
#include "APIWrapper.h"

using namespace std;

/*
 * 
 */
int main() {
    string username;
    string password;
    cout << "Enter your username: ";
    cin >> username;
	cout << "Enter your password: ";
    cin >> password;
    
    APIWrapper api (username, password);
    api.logOut();
    
    return 0;
}

