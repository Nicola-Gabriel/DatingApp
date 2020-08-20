import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegisterMode = new EventEmitter();
  model: any = {};
  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
   return this.authService.register(this.model).subscribe(() => {
      this.alertify.success('registration succesfull');
    }, error => {
      this.alertify.error('user already exist');
    });
  }

  cancel() {
    this.cancelRegisterMode.emit(false);
    this.alertify.message('cancelled');
  }

}
