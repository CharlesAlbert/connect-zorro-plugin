// TestOpenApiLibNative.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <objbase.h>
#include <stdio.h>
#include "WTypes.h"

#using <mscorlib.dll>

using namespace System;
extern "C" int mainCRTStartup();
extern "C" int BrokerLogin(char* User, char* Pwd, char* Type, char* Account);

// Called by mainCRTStartup
int main() {}

[STAThread]
int myMain()
{
	// Initialize the CRT
	mainCRTStartup();

	BrokerLogin(NULL, NULL, NULL, NULL);
    return 0;
}

