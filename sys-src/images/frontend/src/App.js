import React from 'react';
import './App.css';

class App extends React.Component {
    constructor(props) {
      super(props);

      this.backendUrl = process.env.BACKEND_SERVER
      this.backendUrl = 'csharp-backend'
      this.backendUrl = 'http://localhost'
      
      this.state = {
          matchId: '6049289816',
          model: 'kda',
          is_training: false,
          trained: false,
          score: null
      }

      this.handleSubmit = this.handleSubmit.bind(this);
      this.form = React.createRef();

      this.trainModel();
  }

  render() {
    let content = (
      <div className="App">
        <header className="App-header">
          {/* See: https://giphy.com/gifs/3d-animation-dota-2-bluespace-eHKM1zH4JBMk */}
          <img src="https://media.giphy.com/media/eHKM1zH4JBMk/giphy.gif" alt="logo" />

          <div className="row">
              <div className="col-md-6 mb-3">
                  <label htmlFor="matchId">Match-ID</label>
                  <input type="text" className="form-control" id="matchId" required value={this.state.matchId} onChange={this.handleChange} />
              </div>
          </div>

          <form ref={this.form} className="btn-block" onSubmit={this.handleSubmit}>
              <button id="submitbutton" className="btn btn-primary btn-lg btn-block" type="submit">
                  Predict
              </button>
          </form>
          {
            this.state.score &&
            <div className="row">
              <span>Score: {this.state.score}</span>
            </div>
          }
        </header>
      </div>
    );

    return content;
  }

  handleChange = (e) => { 
    this.setState({
        [e.target.id]: e.target.value
    })
  }

  trainModel() {
    if(!this.state.trained && !this.state.is_training) {
      this.setState({ is_training: true})
      console.log("trainModel")
      console.log(`${this.backendUrl}/api/Jupyter/trainmodel?model_name=${encodeURIComponent(this.state.model)}`)

      fetch(`${this.backendUrl}/api/Jupyter/trainmodel?model_name=${encodeURIComponent(this.state.model)}`)
      .then(res => {
        if (!res.ok) {
          console.log("ERROR - Traing again")
          this.trainModel()
        }

        this.setState({ trained: true})
        this.setState({ is_training: false})
      })
    }
  }

  handleSubmit(event) {
    console.log(`${this.backendUrl}/api/Jupyter/predict?matchId=${encodeURIComponent(this.state.matchId)}&model_name=${encodeURIComponent(this.state.model)}`)
    fetch(`${this.backendUrl}/api/Jupyter/predict?matchId=${encodeURIComponent(this.state.matchId)}&model_name=${encodeURIComponent(this.state.model)}`)
    .then(res => {
      if (!res.ok) {
          console.log("ERROR")
          return
      }

      return res.json()
    })
    .then((data) => {
      console.log(data)
      if (data !== undefined) {
        this.setState({ score: data.score })
      } else {
        this.setState({ score: null })
      }
    })

    event.preventDefault();
  }

}

export default App;
