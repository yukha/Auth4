import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-in',
  template: ''
})
export class SignInComponent {

  constructor(router: Router) {
    var url = sessionStorage.getItem('URL_BEFORE_401');
    sessionStorage.removeItem('URL_BEFORE_401');
    router.navigateByUrl(url); // TODO: muze pusobit nekonecnou smycku, pokud service nebezi.
   }
}
