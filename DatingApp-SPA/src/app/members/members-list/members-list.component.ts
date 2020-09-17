import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.css']
})
export class MembersListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];
  userParams: any = {};
  pagination: Pagination;
  constructor(private userService: UserService, private alertify: AlertifyService,
     private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe( data => {
      this.users = data ['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  resetFilters() {
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.loadUsers();
  }

  loadUsers() {
     this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage,
       this.userParams)
        .subscribe((res: PaginatedResult<User[]>) => {
       this.users = res.result;
       this.pagination = res.pagination;
    }, error => {
      this.alertify.error(error);
    });
  }

}
