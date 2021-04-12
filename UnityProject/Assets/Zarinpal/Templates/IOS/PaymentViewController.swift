//
//  PaymentViewController.swift
//  ZarinPalSDKPayment
//
//  Created by ImanX on 12/9/17.
//  Copyright © 2017 ImanX. All rights reserved.
//

import Foundation
import UIKit
import WebKit
class PaymentViewController: UIViewController , UIWebViewDelegate{
    
    var zarinpalUnity:ZarinpalUnity!;
    var authority:String!;
    
    @IBOutlet weak var webKit: UIWebView!
    @IBOutlet weak var indicator: UIActivityIndicatorView!
    @IBOutlet weak var lblTitle: UILabel!
    
    
    override func viewDidLoad() {
        super.viewDidLoad();
        
        
        if let color = zarinpalUnity.zarinpal.indicatorColor {
            self.indicator.color = color;
        }
        
        if let backgroundColor = zarinpalUnity.zarinpal.pageBackgroundColor {
            self.view.backgroundColor = backgroundColor;
        }
        
        if let title = zarinpalUnity.zarinpal.title {
            self.lblTitle.text = title;
            self.lblTitle.isHidden = false;
        }
        
        self.webKit.delegate = self;
        
        let url = URL(string: URLs.START_PG_URL(self.authority));
        self.webKit.loadRequest(URLRequest(url: url!));
        return;
    }
    
    func dismiss() {
        DispatchQueue.main.async {
            self.dismiss(animated: true, completion: {
                print("dismiss");
            })
        }
    }
    
    
    func webViewDidFinishLoad(_ webView: UIWebView) {
        indicator.isHidden = true;
        webKit.isHidden = false;
    }
    
    
    func webView(_ webView: UIWebView, shouldStartLoadWith request: URLRequest, navigationType: UIWebView.NavigationType) -> Bool {
        
        let callback = request.url;
        
        
        guard let host = callback?.host else{
            return false;
        }
        
        guard let scheme = callback?.scheme else {
            return false;
        }
        
        print("scheme : \(scheme) && host : \(host) && url : \(callback?.absoluteString)");
        
        let status = callback?.parse(query: "Status");
        let isStatusNull = status==nil;
        if (!isStatusNull)//scheme + "://" + host )  == zarinpalUnity.callbackUrl
        {

            let isOK = callback?.parse(query: "Status") == "OK";
            let authority = callback?.parse(query: "Authority");
            
            if(!isOK){
                self.zarinpalUnity.onPurchaseFailed(error:"Status not OK");
                self.dismiss();
            }
            else{
                self.zarinpalUnity.onPurchaseSucceeded(authority:authority!,amount:self.zarinpalUnity.zarinpal.amount);
                self.dismiss();
            }
        }
        
        return true;
    }
    
    
    
    @IBAction func closeClicked(_ sender: UIBarButtonItem){
        self.zarinpalUnity.onPurchaseFailed(error: "Purchase canceled");
        self.dismiss();
    }
    
    
    
}


extension URL {
    
    func parse(query:String) -> String? {
        if let uri = URLComponents(string: self.absoluteString){
            return uri.queryItems?.first(where: {$0.name == query})?.value;
        }
        
        return nil;
    }
    
}
