import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SocialService } from '@delon/auth';
import { SettingsService } from '@delon/theme';

@Component({
  selector: 'app-callback',
  template: ``,
  providers: [SocialService]
})
export class CallbackComponent implements OnInit {
  type = '';

  private socialService = inject(SocialService);
  private settingsSrv = inject(SettingsService);
  private route = inject(ActivatedRoute);

  ngOnInit(): void {
    this.type = this.route.snapshot.params['type'];
    this.mockModel();
  }

  private mockModel(): void {
    const info = {
      token: '123456789',
      name: 'cipchk',
      email: `${this.type}@${this.type}.com`,
      id: 10000,
      time: +new Date()
    };
    this.settingsSrv.setUser({
      ...this.settingsSrv.user,
      ...info
    });
    this.socialService.callback(info);
  }
}
