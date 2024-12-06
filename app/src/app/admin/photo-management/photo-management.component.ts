import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { Photo } from '../../_models/photo.model';

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
    this.getPhotosForApproval('Saundra');
  }

  getPhotosForApproval(username: string) {
    this.adminService.getPhotosForApproval(username).subscribe({
      next: (photos) => console.log(photos),
    });
  }

  moderatePhoto(username: string, photoId: string, action: string) {
    this.adminService.moderatePhoto(username, photoId, action).subscribe({
      next: (success) => console.log(success),
    });
  }
}
