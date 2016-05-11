// connect-zorro-plugin.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

BOOL APIENTRY DllMain(
	HANDLE hModule,
	DWORD ul_reason_for_call,
	LPVOID lpReserved)
{
	return TRUE;
}

////////////////////////////////////////////////////////////////
typedef double DATE;
#include "include/trading.h"  // enter your path to trading.h (in your Zorro folder)

#define PLUGIN_VERSION	2
#define DLLFUNC extern "C" __declspec(dllexport)
#define CONNECTED false

////////////////////////////////////////////////////////////////

int(__cdecl *BrokerError)(const char *txt) = NULL;
int(__cdecl *BrokerProgress)(const int percent) = NULL;

////////////////////////////////////////////////////////////////
DLLFUNC int BrokerOpen(char* Name, FARPROC fpError, FARPROC fpProgress)
{
	strcpy_s(Name, 32, "cTrader");
	(FARPROC&)BrokerError = fpError;
	(FARPROC&)BrokerProgress = fpProgress;
	return PLUGIN_VERSION;
}

////////////////////////////////////////////////////////////////
// 0 = test, 1 = relogin, 2 = login, -1 = logout
DLLFUNC int BrokerLogin(char* User, char* Pwd, char* Type, char* Account)
{
	return 0;
}

////////////////////////////////////////////////////////////////
DLLFUNC int BrokerHistory(char* Asset, DATE tStart, DATE tEnd, int nTickMinutes, int nTicks, TICK* ticks)
{
	if (!CONNECTED || !Asset || !ticks || !nTicks) return 0;
	return 0;
}

/////////////////////////////////////////////////////////////////////
DLLFUNC int BrokerTime(DATE *pTimeGMT)
{
	if (!CONNECTED) return 0;
	return 0;
}

DLLFUNC int BrokerAsset(char* Asset, double* pPrice, double* pSpread,
	double *pVolume, double *pPip, double *pPipCost, double *pMinAmount,
	double *pMargin, double *pRollLong, double *pRollShort)
{
	if (!CONNECTED) return 0;
	return 0;
}

DLLFUNC int BrokerAccount(char* Account, double *pdBalance, double *pdTradeVal, double *pdMarginVal)
{
	if (!CONNECTED) return 0;
	return 0;
}

DLLFUNC int BrokerBuy(char* Asset, int nAmount, double dStopDist, double *pPrice)
{
	if (!CONNECTED) return 0;
	return 0;
}

// returns negative amount when the trade was closed
DLLFUNC int BrokerTrade(int nTradeID, double *pOpen, double *pClose, double *pRoll, double *pProfit)
{
	if (!CONNECTED) return -1;
	return -1;
}

DLLFUNC int BrokerSell(int nTradeID, int nAmount)
{
	if (!CONNECTED) return 0;
	return 0;
}

