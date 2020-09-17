import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import {  Router } from '@angular/router';
import { MembersListComponent } from '../members/members-list/members-list.component';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  constructor(private authService: AuthService, private alertify: AlertifyService,
    private route: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }
  login() {
   this.authService.login(this.model).subscribe(next => {
     this.route.navigate(['/members']);
     this.alertify.success('logged in successfuly');
   }, error =>
   this.alertify.warning('user does not exist'));
  }

  loggedIn() {
   return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.route.navigate(['/home']);
    this.alertify.message('logged out');
  }
}
