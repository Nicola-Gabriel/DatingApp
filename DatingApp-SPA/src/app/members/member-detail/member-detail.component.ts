import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  constructor(private userService: UserService, private alertify: AlertifyService,
     private routes: ActivatedRoute) { }

  ngOnInit() {
   this.routes.data.subscribe(data => {
     this.user = data ['user'];
   });

   this.galleryOptions =
    [
      {
          width: '500px',
          height: '600px',
          imagePercent: 100,
          thumbnailsColumns: 5,
          imageAnimation: NgxGalleryAnimation.Slide
      }
   ];
   
   this.galleryImages = this.getImages();

  }

  getImages() {
    const images = [];

    for (let i = 0; i < this.user.photos.length; i++) {
      images.push({
        small: this.user.photos[i].url,
        medium: this.user.photos[i].url,
        big: this.user.photos[i].url,
        description: this.user.photos[i].description
      });
    }
    return images;
  }

   
  /* loadUser() {
    this.userService.getUser(+this.routes.snapshot.params['id']).subscribe((user: User) => {
      this.user = user;
    }, error => {
      this.alertify.error(error);
    } );
  } */

}
