import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { Photo } from '../../_models/photo.model';
import { filter } from 'rxjs';

@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css',
})
export class PhotoManagementComponent implements OnInit {
  private adminService = inject(AdminService);

  photos = signal<Photo[]>([]);

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: (photos) => {
        photos.forEach((p) => {
          this.photos().push({
            id: p.photoId || 0,
            userName: p.userName,
            isApproved: p.isApproved,
            isMain: p.isMain,
            url: p.url,
          } as Photo);
        });
      },
    });
  }

  moderatePhoto(photo: Photo, action: string) {
    this.adminService.moderatePhoto(photo, action).subscribe({
      next: (success) => {
        if (success) {
          this.photos.set(this.photos().filter((p) => p.id != photo.id));
        }
      },
    });
  }
}
