// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: recently_played.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Uplay.RecentlyPlayed {

  /// <summary>Holder for reflection information generated from recently_played.proto</summary>
  public static partial class RecentlyPlayedReflection {

    #region Descriptor
    /// <summary>File descriptor for recently_played.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RecentlyPlayedReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiF1cGNfZHVtcF8xMC9yZWNlbnRseV9wbGF5ZWQucHJvdG8SG21nLnByb3Rv",
            "Y29sLnJlY2VudGx5X3BsYXllZCJDCg9HYW1lU2Vzc2lvbkluZm8SEQoJcHJv",
            "ZHVjdElkGAEgAigNEh0KFXNlc3Npb25FbmRlZFRpbWVzdGFtcBgCIAIoBCJS",
            "ChNSZWNlbnRseVBsYXllZEdhbWVzEjsKBWdhbWVzGAEgAygLMiwubWcucHJv",
            "dG9jb2wucmVjZW50bHlfcGxheWVkLkdhbWVTZXNzaW9uSW5mbw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Uplay.RecentlyPlayed.GameSessionInfo), global::Uplay.RecentlyPlayed.GameSessionInfo.Parser, new[]{ "ProductId", "SessionEndedTimestamp" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Uplay.RecentlyPlayed.RecentlyPlayedGames), global::Uplay.RecentlyPlayed.RecentlyPlayedGames.Parser, new[]{ "Games" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class GameSessionInfo : pb::IMessage<GameSessionInfo>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<GameSessionInfo> _parser = new pb::MessageParser<GameSessionInfo>(() => new GameSessionInfo());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<GameSessionInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Uplay.RecentlyPlayed.RecentlyPlayedReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GameSessionInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GameSessionInfo(GameSessionInfo other) : this() {
      _hasBits0 = other._hasBits0;
      productId_ = other.productId_;
      sessionEndedTimestamp_ = other.sessionEndedTimestamp_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public GameSessionInfo Clone() {
      return new GameSessionInfo(this);
    }

    /// <summary>Field number for the "productId" field.</summary>
    public const int ProductIdFieldNumber = 1;
    private readonly static uint ProductIdDefaultValue = 0;

    private uint productId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint ProductId {
      get { if ((_hasBits0 & 1) != 0) { return productId_; } else { return ProductIdDefaultValue; } }
      set {
        _hasBits0 |= 1;
        productId_ = value;
      }
    }
    /// <summary>Gets whether the "productId" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProductId {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "productId" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProductId() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "sessionEndedTimestamp" field.</summary>
    public const int SessionEndedTimestampFieldNumber = 2;
    private readonly static ulong SessionEndedTimestampDefaultValue = 0UL;

    private ulong sessionEndedTimestamp_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong SessionEndedTimestamp {
      get { if ((_hasBits0 & 2) != 0) { return sessionEndedTimestamp_; } else { return SessionEndedTimestampDefaultValue; } }
      set {
        _hasBits0 |= 2;
        sessionEndedTimestamp_ = value;
      }
    }
    /// <summary>Gets whether the "sessionEndedTimestamp" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSessionEndedTimestamp {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "sessionEndedTimestamp" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSessionEndedTimestamp() {
      _hasBits0 &= ~2;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as GameSessionInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(GameSessionInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ProductId != other.ProductId) return false;
      if (SessionEndedTimestamp != other.SessionEndedTimestamp) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (HasProductId) hash ^= ProductId.GetHashCode();
      if (HasSessionEndedTimestamp) hash ^= SessionEndedTimestamp.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (HasProductId) {
        output.WriteRawTag(8);
        output.WriteUInt32(ProductId);
      }
      if (HasSessionEndedTimestamp) {
        output.WriteRawTag(16);
        output.WriteUInt64(SessionEndedTimestamp);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (HasProductId) {
        output.WriteRawTag(8);
        output.WriteUInt32(ProductId);
      }
      if (HasSessionEndedTimestamp) {
        output.WriteRawTag(16);
        output.WriteUInt64(SessionEndedTimestamp);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (HasProductId) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ProductId);
      }
      if (HasSessionEndedTimestamp) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(SessionEndedTimestamp);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(GameSessionInfo other) {
      if (other == null) {
        return;
      }
      if (other.HasProductId) {
        ProductId = other.ProductId;
      }
      if (other.HasSessionEndedTimestamp) {
        SessionEndedTimestamp = other.SessionEndedTimestamp;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            ProductId = input.ReadUInt32();
            break;
          }
          case 16: {
            SessionEndedTimestamp = input.ReadUInt64();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            ProductId = input.ReadUInt32();
            break;
          }
          case 16: {
            SessionEndedTimestamp = input.ReadUInt64();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class RecentlyPlayedGames : pb::IMessage<RecentlyPlayedGames>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<RecentlyPlayedGames> _parser = new pb::MessageParser<RecentlyPlayedGames>(() => new RecentlyPlayedGames());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<RecentlyPlayedGames> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Uplay.RecentlyPlayed.RecentlyPlayedReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public RecentlyPlayedGames() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public RecentlyPlayedGames(RecentlyPlayedGames other) : this() {
      games_ = other.games_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public RecentlyPlayedGames Clone() {
      return new RecentlyPlayedGames(this);
    }

    /// <summary>Field number for the "games" field.</summary>
    public const int GamesFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Uplay.RecentlyPlayed.GameSessionInfo> _repeated_games_codec
        = pb::FieldCodec.ForMessage(10, global::Uplay.RecentlyPlayed.GameSessionInfo.Parser);
    private readonly pbc::RepeatedField<global::Uplay.RecentlyPlayed.GameSessionInfo> games_ = new pbc::RepeatedField<global::Uplay.RecentlyPlayed.GameSessionInfo>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::Uplay.RecentlyPlayed.GameSessionInfo> Games {
      get { return games_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as RecentlyPlayedGames);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(RecentlyPlayedGames other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!games_.Equals(other.games_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= games_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      games_.WriteTo(output, _repeated_games_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      games_.WriteTo(ref output, _repeated_games_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      size += games_.CalculateSize(_repeated_games_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(RecentlyPlayedGames other) {
      if (other == null) {
        return;
      }
      games_.Add(other.games_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            games_.AddEntriesFrom(input, _repeated_games_codec);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            games_.AddEntriesFrom(ref input, _repeated_games_codec);
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code