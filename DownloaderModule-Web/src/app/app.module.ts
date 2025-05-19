import { NgModule } from "@angular/core";
import { AppComponent } from "./app.component";
import { BrowserModule } from "@angular/platform-browser";
import { RouterOutlet } from "@angular/router";
import { HomePageModule } from "./components/home-page/home-page.module";
import { HomePageService } from "./services/home-page.service";
import { HttpClientModule } from "@angular/common/http";


@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, RouterOutlet, HomePageModule, HttpClientModule],
  providers: [HomePageService],
  bootstrap: [AppComponent]
})

export class AppModule { }