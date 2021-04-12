//
//  ZarinpalUnityWrapper.swift
//  ZarinpalUnityPlugin
//
//  Created by Mojtaba Pirveisi on 3/16/19.
//  Copyright © 2019 Mojtaba Pirveisi. All rights reserved.
//

import Foundation

@objc public class ZarinpalUnityWrapper: NSObject {
    
    
    @objc public static let shared = ZarinpalUnityWrapper();
    
    private let zarinpalUnity = ZarinpalUnity();
    
    @objc public func initialize(merchantID : String,callbackUrl : String ,autoVerifyPurchase : Bool,autoStartPurchase:Bool)
    {
        zarinpalUnity.initialize(merchantID: merchantID, callbackUrl: callbackUrl, autoVerifyPurchase: autoVerifyPurchase, autoStartPurchase: autoStartPurchase);
    }
    
    @objc public func startPurchaseFlow(amount : Int , productID: String , desc: String) {
        
        zarinpalUnity.startPurchase(amount: amount, productID: productID, desc: desc);
    }
    
    @objc public func purchase(authority:String) {
        
        zarinpalUnity.openPaymentGateWay(authority: authority);
    }
    
    @objc public func verifyPurchase(authority:String,amount:Int) {
        
        zarinpalUnity.verifyPurchase(authority: authority, amount: amount);
    }
}
