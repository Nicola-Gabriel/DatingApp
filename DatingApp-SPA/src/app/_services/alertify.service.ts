import { Injectable } from '@angular/core';
declare let alertify: any;
@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

constructor() { }

confirm(message: string, okCallBack: () => any ) {
  alertify.confirm(message, function(e) {
    if (e) {
      okCallBack();
    } else {}
  });
}

warning(message: string) {
  alertify.warning(message);
}

success(message: string) {
  alertify.success(message);
}

error(message: string) {
  alertify.error(message);
}

message(message: string) {
  alertify.message(message);
}

}
