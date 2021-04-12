//
//  ZarinpalUnityBridge.m
//  ZarinpalUnityPlugin
//
//  Created by Mojtaba Pirveisi on 3/16/19.
//  Copyright © 2019 Mojtaba Pirveisi. All rights reserved.
//

#import <Foundation/Foundation.h>
#include "ZarinpalUnityPlugin-Swift.h"



#pragma mark - C interface

extern "C" {
    
    void _zu_initialize(const char* merchantID,const char* callbackUrl,const bool autoVerifyPurchase,const bool autoStartPurchase)
    {
        [[ZarinpalUnityWrapper shared] initializeWithMerchantID:[NSString stringWithUTF8String:merchantID] callbackUrl:[NSString stringWithUTF8String:callbackUrl]  autoVerifyPurchase:autoVerifyPurchase autoStartPurchase:autoStartPurchase];
    }
    
    void _zu_startPurchase(int amount,const char* productID,const char* desc)
    {
        [[ZarinpalUnityWrapper shared] startPurchaseFlowWithAmount:amount productID:[NSString stringWithUTF8String:productID] desc:[NSString stringWithUTF8String:desc]];
    }
    
    void _zu_openPaymentGateway (const char* authority)
    {
        [[ZarinpalUnityWrapper shared] purchaseWithAuthority:[NSString stringWithUTF8String:authority]];
    }
    
    void _zu_verifyPurchase(const char* authority,int amount)
    {
        [[ZarinpalUnityWrapper shared] verifyPurchaseWithAuthority:[NSString stringWithUTF8String:authority] amount:amount];
    }
    
}
