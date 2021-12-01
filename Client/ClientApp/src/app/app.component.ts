import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  current_title = 'Client';

  constructor(private titleService: Title, private router: Router, private activatedRoute: ActivatedRoute, private translate: TranslateService) {
    translate.setDefaultLang('en');
  }

  ngOnInit() {
    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
      map(() => {
        return this.activatedRoute.firstChild.snapshot.data['title'];
      })
    ).subscribe((title: string) => {
      this.titleService.setTitle(title);
      this.current_title = title
    });
  }

}
