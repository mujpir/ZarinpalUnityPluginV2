//
//  ZarinpalUnity.swift
//  ZarinpalUnityPluginXcode
//
//  Created by Mojtaba Pirveisi on 3/16/19.
//  Copyright © 2019 Mojtaba Pirveisi. All rights reserved.
//


import UIKit

class ZarinpalUnity {
    
    
    let kCallbackTarget = "ZarinpaliOS"
    var merchantID : String = "" ;
    var callbackUrl : String = "";
    var autoVerifyPurchase : Bool = false;
    var autoStartPurchase : Bool = false;
    var viewController : UIViewController?;
    var zarinpal:Builder!;
    
    func initialize(merchantID:String,callbackUrl : String,autoVerifyPurchase : Bool,autoStartPurchase:Bool)
    {
        
        self.merchantID = merchantID ;
        self.callbackUrl = callbackUrl;
        self.autoVerifyPurchase = autoVerifyPurchase;
        self.autoStartPurchase = autoStartPurchase;
        viewController = UIApplication.shared.keyWindow!.rootViewController;
        UnitySendMessage(kCallbackTarget,"OnStoreInitialized","nullMessages");
    }
    
    func startPurchase(amount : Int , productID : String , desc : String) {
        zarinpal = Builder(vc: viewController!, merchantID: self.merchantID, amount:amount, description: desc,callbackUrl: self.callbackUrl,autoVerifyPurchase: self.autoVerifyPurchase,autoStartPurchase:self.autoStartPurchase);
        
        zarinpal.indicatorColor = UIColor.black;  //this set indicator color *optional
        zarinpal.title = "Payment Gateway";  //this set title of payment page *optional
        zarinpal.description = desc;  //this set title of payment page *optional
        zarinpal.pageBackgroundColor = UIColor.lightGray; // this set background payment color *optional
        zarinpal.email = "email@gmail.com"; //this set email *optional
        zarinpal.mobile = "09199022505"; //this set mobile *optional
        
        let request = HttpRequest(url: URLs.PAYMENT_REQUEST_URL(), method: .Post);
        
        request.params = [
            "merchant_id" : zarinpal.merchantID,
            "amount" : zarinpal.amount,
            "description" : zarinpal.description,
            "callback_url" : zarinpal.callbackUrl
        ];
        
        request.request { (response) in
            if let data = response["data"] as? [String: Any]
            {
                let code = data["code"] as! Int;
                if code == 100 {
                    let authority = data["authority"] as! String;
                    self.onPurchaseStarted(authority: authority);
                }
                else
                {
                    self.onPurchaseFailedToStart(error:"Error on new purchase :Status \(code) ");
                }
            }
            else
            {
                let error = response["errors"] as! [String: Any]
                self.onPurchaseFailedToStart(error:"Error on new purchase :Status \(error["code"]) ");
            }

        }
    }
    
    func openPaymentGateWay(authority:String) {
        let storyboard = UIStoryboard(name: "PaymentBoard", bundle: Bundle(for: PaymentViewController.self));
        let vc = storyboard.instantiateViewController(withIdentifier: "PaymentViewController") as! PaymentViewController;
        vc.zarinpalUnity = self;
        vc.authority = authority;
        self.viewController?.present(vc, animated: true, completion: nil);
    }
    
    
    //Todo change amount and merchant when calling just verification
    func verifyPurchase(authority:String,amount:Int) {
        let request = HttpRequest(url: URLs.VERIFICATION_URL(), method: .Post);
        
        request.params = [
            "merchant_id" : merchantID,
            "amount" : amount,
            "authority" : authority,
        ];
        
        request.request { (response) in
            if let data = response["data"] as? [String: Any]
            {
                let code = data["code"] as! Int;
                if code == 100 {
                    let refID = data["ref_id"] as! Int;
                    self.onVerificationSucceeded(refID: refID.description, authority: authority);
                    return;
                }
                
                self.onVerificationFailed(code: code, authority: authority);
            }
            else
            {
                let error = response["errors"] as! [String: Any]
                let code = error["code"] as! Int;
                self.onVerificationFailed(code:code, authority: authority);
            }

        }
    }
    
    
    //Call Backs
    
    //On Purchse Started
    func onPurchaseStarted(authority: String) {
        UnitySendMessage(kCallbackTarget,"OnPurchaseStarted",authority);
        if(autoStartPurchase){
            DispatchQueue.main.async {
                self.openPaymentGateWay(authority: authority);
            }

        }
        print("On Purchase Started : \(authority)");
    }
    
    func onPurchaseFailedToStart(error : String) {
        var error : String = "Unexpected error on starting a purchase : \(error)";
        UnitySendMessage(kCallbackTarget,"OnPurchaseFailedToStart",error);
        print(error)
    }
    
    func onPurchaseSucceeded(authority:String,amount:Int) {
        UnitySendMessage(kCallbackTarget,"OnPurchaseSucceeded",authority);
        if(autoVerifyPurchase){
            verifyPurchase(authority: authority,amount:amount);
        }
    }
    
    func onPurchaseFailed(error:String) {
        UnitySendMessage(kCallbackTarget,"OnPurchaseFailed",error);
    }
    
    func onVerificationSucceeded(refID: String, authority: String) {
        //when Payment is Success and return:
        //refID: this is transaction id.
        //authority: this is a payment unique id
        //payment : included payment details ex: amount , description
        print(refID);
        var message : String = "Payment is success with refid : \(refID)";
        let jsonObject : NSMutableDictionary = NSMutableDictionary();
        jsonObject.setValue(authority, forKey: "authority");
        jsonObject.setValue(refID, forKey: "refid");
        
        let jsonData :NSData;
        
        do{
            jsonData = try JSONSerialization.data(withJSONObject: jsonObject, options: (JSONSerialization.WritingOptions())) as NSData;
            let jsonString = NSString(data:jsonData as Data,encoding:String.Encoding.utf8.rawValue) as! String;
            print("purchase verification json : \(jsonString)");
            UnitySendMessage(kCallbackTarget,"OnPaymentVerificationSucceed",jsonString);
        }
        catch
        {
            print("exception at json11");
        }
    }
    
    func onVerificationFailed(code: Int, authority: String?) {
        //when Payment is failure and return:
        //status : ZarinPal failure codes
        //authority: this is a payment unique id
        
        var error : String = "Purchase failed with code : \(code)";
        print(error);
        UnitySendMessage(kCallbackTarget,"OnPurchaseFailed",String(error));
    }
    
    
    open class Builder{
        
        public var viewController:UIViewController!;
        public var autoVerifyPurchase:Bool;
        public var autoStartPurchase : Bool;
        public var merchantID:String!;
        public var amount:Int!;
        public var description:String!;
        public var mobile:String?;
        public var email:String?;
        public var callbackUrl:String?
        
        public var indicatorColor:UIColor?;
        public var pageBackgroundColor:UIColor?;
        public var title:String?;
        
        public init(vc:UIViewController , merchantID:String , amount:Int , description:String,
                    callbackUrl:String,autoVerifyPurchase:Bool,autoStartPurchase : Bool){
            self.viewController = vc;
            self.merchantID = merchantID;
            self.amount = amount;
            self.description = description;
            self.callbackUrl = callbackUrl;
            self.autoVerifyPurchase = autoVerifyPurchase;
            self.autoStartPurchase = autoStartPurchase;
        }
    }
    
}
