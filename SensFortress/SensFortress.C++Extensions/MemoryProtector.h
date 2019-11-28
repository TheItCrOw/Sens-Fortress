#pragma once
//class MemoryProtector
//{
//#include <windows.h>
//#include <stdio.h>
//#include <Wincrypt.h>
//
//#define SSN_STR_LEN 12  // includes null
//
//	void main()
//	{
//		HRESULT hr = S_OK;
//		LPWSTR pSensitiveText = NULL;
//		DWORD *cbSensitiveText = 0;
//		DWORD cbPlainText = SSN_STR_LEN * sizeof(WCHAR);
//		DWORD *dwMod = 0;
//
//		//  Memory to encrypt must be a multiple of CRYPTPROTECTMEMORY_BLOCK_SIZE.
//		if (dwMod = cbPlainText % CRYPTPROTECTMEMORY_BLOCK_SIZE)
//			cbSensitiveText = cbPlainText +
//			(CRYPTPROTECTMEMORY_BLOCK_SIZE - dwMod);
//		else
//			cbSensitiveText = cbPlainText;
//
//		pSensitiveText = (LPWSTR)LocalAlloc(LPTR, cbSensitiveText);
//		if (NULL == pSensitiveText)
//		{
//			wprintf(L"Memory allocation failed.\n");
//			return E_OUTOFMEMORY;
//		}
//
//		//  Place sensitive string to encrypt in pSensitiveText.
//
//		if (!CryptProtectMemory(pSensitiveText, cbSensitiveText,
//			CRYPTPROTECTMEMORY_SAME_PROCESS))
//		{
//			wprintf(L"CryptProtectMemory failed: %d\n", GetLastError());
//			SecureZeroMemory(pSensitiveText, cbSensitiveText);
//			LocalFree(pSensitiveText);
//			pSensitiveText = NULL;
//			return E_FAIL;
//		}
//
//		//  Call CryptUnprotectMemory to decrypt and use the memory.
//
//		SecureZeroMemory(pSensitiveText, cbSensitiveText);
//		LocalFree(pSensitiveText);
//		pSensitiveText = NULL;
//
//		return hr;
//	}
//};

