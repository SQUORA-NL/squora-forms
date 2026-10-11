using System.Text.RegularExpressions;
using Squora.Forms.Hub.Api.Models.Collaboration;
using MongoDB.Driver;

namespace Squora.Forms.Hub.Api.Services.Collaboration;

public class CommentService
{
    private readonly IMongoCollection<Comment> _comments;
    private readonly IMongoCollection<ActivityEvent> _events;

    public CommentService(IMongoDatabase database)
    {
        _comments = database.GetCollection<Comment>("comments");
        _events = database.GetCollection<ActivityEvent>("activity_events");
        EnsureIndexes();
    }

    public async Task<Comment> CreateThreadAsync(string workspaceId, string definitionId,
        string anchorPath, string authorUserId, string body)
    {
        return await InsertCommentAsync(workspaceId, definitionId, anchorPath, authorUserId, body, null);
    }

    public async Task<Comment> ReplyAsync(string workspaceId, string definitionId,
        string anchorPath, string authorUserId, string body, string parentCommentId)
    {
        return await InsertCommentAsync(workspaceId, definitionId, anchorPath, authorUserId, body, parentCommentId);
    }

    public async Task<List<Comment>> GetByDefinitionAsync(string definitionId)
    {
        return await _comments.Find(c => c.DefinitionId == definitionId)
            .SortBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task ResolveAsync(string commentId)
    {
        await SetStatusAsync(commentId, "resolved");
    }

    public async Task ReopenAsync(string commentId)
    {
        await SetStatusAsync(commentId, "open");
    }

    public async Task DeleteAsync(string commentId)
    {
        await _comments.DeleteOneAsync(c => c.Id == commentId);
    }

    private async Task<Comment> InsertCommentAsync(string workspaceId, string definitionId,
        string anchorPath, string authorUserId, string body, string? parentId)
    {
        ValidateBody(body);
        var comment = new Comment
        {
            WorkspaceId = workspaceId,
            DefinitionId = definitionId,
            AnchorPath = anchorPath,
            ParentCommentId = parentId,
            AuthorUserId = authorUserId,
            Body = body,
            Mentions = ExtractMentions(body)
        };
        await _comments.InsertOneAsync(comment);
        await EmitEventAsync(workspaceId, definitionId, authorUserId, "comment.added");
        return comment;
    }

    private async Task SetStatusAsync(string commentId, string status)
    {
        await _comments.UpdateOneAsync(
            c => c.Id == commentId,
            Builders<Comment>.Update
                .Set(c => c.Status, status)
                .Set(c => c.UpdatedAt, DateTime.UtcNow));
    }

    private async Task EmitEventAsync(string workspaceId, string definitionId, string actorId, string type)
    {
        await _events.InsertOneAsync(new ActivityEvent
        {
            WorkspaceId = workspaceId,
            DefinitionId = definitionId,
            ActorUserId = actorId,
            Type = type
        });
    }

    private static void ValidateBody(string body)
    {
        if (body.Length > 4000)
            throw new InvalidOperationException("Comment body exceeds 4000 character limit.");
    }

    private void EnsureIndexes()
    {
        _comments.Indexes.CreateOne(new CreateIndexModel<Comment>(
            Builders<Comment>.IndexKeys.Ascending(c => c.DefinitionId).Ascending(c => c.AnchorPath)));
    }

    private static List<string> ExtractMentions(string body)
    {
        return Regex.Matches(body, @"@(\w+)")
            .Select(m => m.Groups[1].Value)
            .ToList();
    }
}
