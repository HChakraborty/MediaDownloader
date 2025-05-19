import { Component } from '@angular/core';
import { HomePageService } from '../../services/home-page.service';

@Component({
  selector: 'app-home-page',
  standalone: false,
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss'
})
export class HomePageComponent {

  constructor(private homePageService: HomePageService) { }

  public buttonVisible = true;

  downloadVideo(videoUrl: string) {
    this.buttonVisible = false;
    this.homePageService.GetVideo(videoUrl).subscribe(response => {
      const blob = response.body!;
      const contentDisposition = response.headers.get('Content-Disposition');

      const fileName =
        contentDisposition?.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/)?.[1]?.replace(/['"]/g, '') ?? 'download.bin';

      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = fileName;
      a.click();
      URL.revokeObjectURL(url);

      this.buttonVisible = true;
    });
  }
}
