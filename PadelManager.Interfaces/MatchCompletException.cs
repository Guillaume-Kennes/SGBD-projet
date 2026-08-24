namespace PadelManager.Interfaces;

// Levée par IMatchRepository.InscrireEtPayerAsync lorsque le match compte déjà 4 participations
// au moment où le verrou est acquis (ENF-010, R-STR-002) : perdu la course à la place.
public class MatchCompletException : Exception {
}
