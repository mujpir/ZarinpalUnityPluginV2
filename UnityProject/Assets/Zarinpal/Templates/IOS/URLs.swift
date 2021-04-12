//
//  URLs.swift
//  ZarinPalSDKPayment
//
//  Created by ImanX on 12/9/17.
//  Copyright © 2017 ImanX. All rights reserved.
//

import Foundation
class URLs {
   static let PAYMENT_REQUEST_URL = {()->String in
    return "https://api.zarinpal.com/pg/v4/payment/request.json";
  }
    
    static let START_PG_URL = {(authority:String)->String in
        return "https://www.zarinpal.com/pg/StartPay/\(authority)";
    }
    
    static let VERIFICATION_URL = {()->String in
        return "https://api.zarinpal.com/pg/v4/payment/verify.json";
    }
}
