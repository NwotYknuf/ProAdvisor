using System;
namespace ProAdvisor.app {
public class Review {

    public Review(DateTime date, string commentaire, double note) {
        this._date = date;
        this._commentaire = commentaire;
        this._note = note;
    }

    private string _commentaire;
    private double _note;
    private DateTime _date;

    public string commentaire {
        get => _commentaire;
    }

    public DateTime date {
        get => _date;
    }

    public double note {
        get => _note;
    }

}
}