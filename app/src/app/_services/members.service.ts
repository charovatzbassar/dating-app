import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member.model';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo.model';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl: string = environment.apiUrl;
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users').subscribe({
      next: (members) => this.members.set(members),
    });
  }

  getMember(username: string) {
    const member = this.members().find((x) => x.username === username);

    if (member !== undefined) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      tap(() => {
        this.members.update((members) =>
          members.map((m) => (m.username === member.username ? member : m))
        );
      }) // do something with the observable without changing it
    );
  }

  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.baseUrl + 'users/set-main-photo/' + photo.id, {})
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((member) => {
              if (member.photos.includes(photo)) {
                member.photoUrl = photo.url;
              }
              return member;
            })
          );
        })
      );
  }

  deletePhoto(photo: Photo) {
    return this.http
      .delete(this.baseUrl + 'users/delete-photo/' + photo.id)
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((member) => {
              if (member.photos.includes(photo)) {
                member.photos = member.photos.filter((p) => p.id !== photo.id);
              }
              return member;
            })
          );
        })
      );
  }
}
