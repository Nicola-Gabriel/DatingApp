import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { MembersListComponent } from '../members/members-list/members-list.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegisterMode = new EventEmitter();
  user: User;
  registerForm: FormGroup;
  constructor(private authService: AuthService, private route: Router,
     private alertify: AlertifyService,
    private fb: FormBuilder) { }

  ngOnInit() {
   this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required,
        Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Registration succesfully');
      }, error => {
        this.alertify.error('User already exist');
    }, () => {
      this.authService.login(this.user).subscribe(() => {
        this.route.navigate(['/members']);
      });
    });
  }
  }

  cancel() {
    this.cancelRegisterMode.emit(false);
    this.alertify.message('cancelled');
  }

}
