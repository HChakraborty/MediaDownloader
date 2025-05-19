import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HomePageService {

  constructor(private http: HttpClient) { }

  public GetVideo(videoUrl: string) : Observable<any> {
    var encodedUrl = encodeURIComponent(videoUrl);
    return this.http.get(`https://localhost:7190/api/VideoDownload/downloadVideo/${encodedUrl}`, {
      responseType: 'blob',
      observe: 'response'
    });
  }
}
