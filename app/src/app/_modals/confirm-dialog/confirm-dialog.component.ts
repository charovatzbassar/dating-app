import { Component, inject } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.css'
})
export class ConfirmDialogComponent {
  bsModalRef = inject(BsModalRef);
  title: string = '';
  message: string = '';
  btnOkText: string = '';
  btnCancelText: string = '';
  result: boolean = false;

  confirm() {
    this.result = true;
    this.bsModalRef.hide();
  }

  decline() {
    this.bsModalRef.hide();
  }
}
